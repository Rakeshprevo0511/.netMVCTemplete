/*!
 * ajaxHelper.js
 * -----------------------------------------------------------------------
 * Centralized AJAX wrapper for every API call in the app. Include this one
 * script tag on a page and you get, for free:
 *
 *   1. Automatic 401 -> remember-me -> refresh -> retry, race-safe.
 *   2. A global loading overlay while any request is in flight.
 *   3. Optional per-button loading/disabled state for the button that
 *      triggered a call.
 *
 * Why this exists
 * ----------------
 * Previously each page (Login.cshtml, _Layout.cshtml, ...) had its own
 * copy-pasted "on 401, check remember-me, then refresh" logic, plus no
 * shared loading UI at all. Bugs that caused individual API calls to bounce
 * to Login even when the session was fine:
 *
 *   1. _Layout.cshtml compared `r.data === true` but the server returns
 *      `Data` (capital D, ApiResponse<T>.Data) — always false, so it went
 *      straight to redirectToLogin() no matter what the server said.
 *
 *   2. No single-flight lock. Two calls 401-ing close together each fired
 *      their own POST /api/admin/refresh. The refresh token is single-use
 *      / rotated server-side, so the first call rotates it and succeeds,
 *      and the second — now holding an already-used token — trips reuse
 *      detection, which revokes *every* session for that user.
 *
 *   3. SetRememberMeCookie() on the server hardcoded Secure = true instead
 *      of following the request scheme like the other auth cookies do —
 *      over plain HTTP the browser silently drops that cookie, so
 *      IsRememberMe() always read false and refresh never even got tried.
 *
 * This module fixes the client half of all that: correct casing, a
 * single-flight refresh lock, and a transparent retry of the original
 * call with no page reload. (The Secure-cookie fix is server-side, in
 * AuthService.SetRememberMeCookie.)
 *
 * Usage
 * -----
 *   ajaxHelper.get("/api/admin/users", { pageNumber: 1 })
 *   ajaxHelper.post("/api/admin/set-content", { id: 1, body: "..." })
 *   ajaxHelper.request({ url: "...", type: "PUT", data: JSON.stringify(x), contentType: "application/json" })
 *
 * All of the above return a jQuery promise. Use .then()/.catch() (or
 * .done()/.fail()) — avoid passing `success`/`error` in the options
 * object, since those fire on the ORIGINAL attempt only and would run
 * before a silent retry ever gets a chance.
 *
 * Options (beyond standard $.ajax options):
 *   - noRedirect: don't auto-navigate to Login on an unrecoverable auth
 *     failure; just reject so the caller can handle it (the Login page
 *     itself does this, since it's already where it would be redirected to).
 *   - button: a jQuery object, element, or selector for the button that
 *     triggered this call. It's disabled and shown a spinner for the
 *     duration of the call (including any silent-refresh retry), then
 *     restored to its original state.
 *   - silent: true to skip the global loading overlay for this one call
 *     (e.g. background polling) — button loading, if any, still applies.
 * -----------------------------------------------------------------------
 */
(function (window, $) {
    "use strict";

    if (!$) {
        throw new Error("ajaxHelper.js requires jQuery to be loaded first.");
    }

    // ---------------------------------------------------------------------
    // Global loading overlay — injected once, used by every page that
    // includes this script. No per-page markup/CSS needed.
    // ---------------------------------------------------------------------
    var LOADER_ID = "ajaxGlobalLoader";
    var activeRequestCount = 0;

    function ensureLoaderInstalled() {
        if (document.getElementById(LOADER_ID)) {
            return;
        }

        var style = document.createElement("style");
        style.textContent =
            "#" + LOADER_ID + "{position:fixed;inset:0;z-index:20000;display:none;" +
            "align-items:center;justify-content:center;background:rgba(255,255,255,.55);}" +
            "#" + LOADER_ID + ".is-visible{display:flex;}" +
            "#" + LOADER_ID + " .ajax-spinner{width:2.75rem;height:2.75rem;border-radius:50%;" +
            "border:.3rem solid rgba(13,110,253,.25);border-top-color:#0d6efd;" +
            "animation:ajaxSpin .6s linear infinite;}" +
            "@keyframes ajaxSpin{to{transform:rotate(360deg);}}" +
            ".ajax-btn-spinner{display:inline-block;width:.9rem;height:.9rem;margin-right:.5rem;" +
            "border-radius:50%;border:.15rem solid rgba(255,255,255,.5);border-top-color:#fff;" +
            "vertical-align:-1px;animation:ajaxSpin .6s linear infinite;}";
        document.head.appendChild(style);

        var overlay = document.createElement("div");
        overlay.id = LOADER_ID;
        overlay.innerHTML = '<div class="ajax-spinner" role="status" aria-label="Loading"></div>';
        document.body.appendChild(overlay);
    }

    function showGlobalLoader() {

        // If startup/auth loader is visible, don't show global loader
        if ($("#authOverlay").is(":visible")) {
            return;
        }

        activeRequestCount++;

        ensureLoaderInstalled();

        document.getElementById(LOADER_ID).classList.add("is-visible");
    }

    function hideGlobalLoader() {

        if ($("#authOverlay").is(":visible")) {
            return;
        }

        activeRequestCount = Math.max(0, activeRequestCount - 1);

        if (activeRequestCount === 0) {
            document.getElementById(LOADER_ID).classList.remove("is-visible");
        }
    }

    // ---------------------------------------------------------------------
    // Per-button loading state
    // ---------------------------------------------------------------------
    function startButtonLoading($btn) {
        if (!$btn || !$btn.length) return;

        // Remember the button's original markup exactly once, even if it's
        // reused for several calls over the page's lifetime.
        if ($btn.data("ajaxOriginalHtml") === undefined) {
            $btn.data("ajaxOriginalHtml", $btn.html());
        }

        $btn.prop("disabled", true);
        $btn.html('<span class="ajax-btn-spinner"></span>' + $btn.data("ajaxOriginalHtml"));
    }

    function stopButtonLoading($btn) {
        if (!$btn || !$btn.length) return;

        var original = $btn.data("ajaxOriginalHtml");
        if (original !== undefined) {
            $btn.html(original);
        }
        $btn.prop("disabled", false);
    }

    // ---------------------------------------------------------------------
    // Auth / refresh
    // ---------------------------------------------------------------------

    // Endpoints that must never be routed back through the refresh/retry
    // flow — refreshing because /refresh itself 401'd would recurse forever.
    var AUTH_ENDPOINTS = [
        "/api/admin/login",
        "/api/admin/refresh",
        "/api/admin/remember-me",
        "/api/admin/logout"
    ];

    // Single-flight lock: while a refresh is in progress, every caller gets
    // the SAME promise back instead of firing its own /refresh request.
    var refreshPromise = null;

    // Guards against redirectToLogin() firing more than once (e.g. three
    // concurrent calls all landing on "session can't be refreshed").
    var loggingOut = false;

    function isAuthEndpoint(url) {
        if (!url) return false;
        return AUTH_ENDPOINTS.some(function (endpoint) {
            return url.indexOf(endpoint) >= 0;
        });
    }

    /**
     * Ensures at most one {remember-me check -> refresh} round-trip is ever
     * running at a time. Returns the shared in-flight promise if one is
     * already active, otherwise starts a new one.
     */
    function ensureSessionRefreshed() {
        if (refreshPromise) {
            return refreshPromise;
        }

        refreshPromise = $.ajax({ url: "/api/admin/remember-me", type: "GET" })
            .then(function (r) {
                var remembered = !!(r && r.Data === true);

                if (!remembered) {
                    return $.Deferred().reject({ reason: "not-remembered" }).promise();
                }

                return $.ajax({ url: "/api/admin/refresh", type: "POST" });
            })
            .always(function () {
                // Release the lock once this attempt fully settles (success
                // or failure) so a future 401 can trigger a fresh attempt.
                refreshPromise = null;
            });

        return refreshPromise;
    }

    /**
     * Revokes the session server-side and sends the user to the login page.
     * Safe to call multiple times — only the first call actually does anything.
     */
    function redirectToLogin() {
        if (loggingOut) {
            return;
        }
        loggingOut = true;

        $.ajax({
            url: "/api/admin/logout",
            type: "POST",
            contentType: "application/json"
        }).always(function () {
            window.location.href = "/Home/Login";
        });
    }

    /**
     * Internal recursive dispatch: does the actual fetch + 401 handling.
     * Deliberately has NO UI side effects (no loader, no button state) so
     * that a silent-refresh retry doesn't double up on those — only the
     * public request() wrapper below touches the UI, exactly once per
     * user-initiated call.
     */
    function dispatch(options, isRetry) {
        return $.ajax(options).catch(function (xhr) {
            var status = xhr && xhr.status;

            // Only intercept 401s from real protected endpoints, and only
            // ever retry ONCE per call (prevents an infinite loop if the
            // server keeps saying 401 even after a "successful" refresh).
            if (status !== 401 || isAuthEndpoint(options.url) || isRetry) {
                return $.Deferred().reject(xhr).promise();
            }

            return ensureSessionRefreshed().then(
                function () {
                    // Session restored — transparently replay the original
                    // request. No page reload, no lost UI state.
                    return dispatch(options, true);
                },
                function () {
                    if (!options.noRedirect) {
                        redirectToLogin();
                    }
                    return $.Deferred().reject(xhr).promise();
                }
            );
        });
    }

    /**
     * Public entry point. Every API call should go through this (directly
     * or via get/post/put/del below) so the 401 -> refresh -> retry
     * behavior, the global loader, and button-loading state are all
     * applied consistently instead of being reimplemented per page.
     */
    function request(options) {
        options = options || {};
        var $btn = options.button ? $(options.button) : null;
        var showLoader = options.silent !== true;

        if (showLoader) showGlobalLoader();
        startButtonLoading($btn);

        return dispatch(options, false).always(function () {
            if (showLoader) hideGlobalLoader();
            stopButtonLoading($btn);
        });
    }

    function get(url, data, opts) {
        return request($.extend({ url: url, type: "GET", data: data }, opts));
    }

    function post(url, data, opts) {
        return request($.extend({
            url: url,
            type: "POST",
            contentType: "application/json",
            data: data !== undefined ? JSON.stringify(data) : undefined
        }, opts));
    }

    function put(url, data, opts) {
        return request($.extend({
            url: url,
            type: "PUT",
            contentType: "application/json",
            data: data !== undefined ? JSON.stringify(data) : undefined
        }, opts));
    }

    function del(url, opts) {
        return request($.extend({ url: url, type: "DELETE" }, opts));
    }

    $(ensureLoaderInstalled);

    window.ajaxHelper = {
        request: request,
        get: get,
        post: post,
        put: put,
        delete: del,
        redirectToLogin: redirectToLogin
    };


})(window, window.jQuery);

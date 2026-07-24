(function (window) {

    "use strict";

    var modalHelper = {};

    modalHelper.show = function (selector) {

        var element = document.querySelector(selector);

        if (!element) return;

        bootstrap.Modal.getOrCreateInstance(element).show();

    };

    modalHelper.hide = function (selector) {

        var element = document.querySelector(selector);

        if (!element) return;

        bootstrap.Modal.getOrCreateInstance(element).hide();

    };

    modalHelper.confirm = function (options) {

        options = options || {};

        $("#confirmTitle").text(options.title || "Confirmation");
        $("#confirmHeading").text(options.heading || "");
        $("#confirmSubHeading").text(options.subHeading || "");

        if (options.html)
            $("#confirmMessage").html(options.message || "");
        else
            $("#confirmMessage").text(options.message || "");

        $("#confirmIcon")
            .attr("class", "bi fs-1 me-3 " + (options.icon || "bi-question-circle-fill text-warning"));

        $("#btnConfirmAction")
            .removeClass()
            .addClass("btn " + (options.buttonClass || "btn-primary"))
            .html((options.confirmIcon ? '<i class="bi ' + options.confirmIcon + ' me-1"></i>' : '') +
                (options.buttonText || "Confirm"));

        $("#btnCancelAction")
            .removeClass()
            .addClass("btn " + (options.cancelClass || "btn-secondary"))
            .html((options.cancelIcon ? '<i class="bi ' + options.cancelIcon + ' me-1"></i>' : '') +
                (options.cancelText || "Cancel"));

        $("#btnConfirmAction")
            .off("click")
            .on("click", function () {

                if (options.showLoader) {

                    $(this)
                        .prop("disabled", true)
                        .html(
                            '<span class="spinner-border spinner-border-sm me-2"></span>' +
                            (options.loaderText || "Processing...")
                        );
                }

                if (options.onConfirm)
                    options.onConfirm();

                if (options.closeOnConfirm !== false) {

                    modalHelper.hide("#confirmModal");

                }


            });

        $("#btnCancelAction")
            .off("click")
            .on("click", function () {

                if (options.onCancel)
                    options.onCancel();

                if (options.closeOnCancel !== false)
                    modalHelper.hide("#confirmModal");

            });
        $("#confirmModal")
            .off("hidden.bs.modal.modalHelper")
            .on("hidden.bs.modal.modalHelper", function () {

                resetConfirmButton();

            });
        if (options.beforeShow)
            options.beforeShow();

        modalHelper.show("#confirmModal");

        if (options.afterShow)
            options.afterShow();
        function resetConfirmButton() {

            $("#btnConfirmAction")
                .prop("disabled", false)
                .removeClass("disabled")
                .html(
                    (options.confirmIcon
                        ? '<i class="bi ' + options.confirmIcon + ' me-1"></i>'
                        : '') +
                    (options.buttonText || "Confirm")
                );

        }
    };

    window.modalHelper = modalHelper;

})(window);
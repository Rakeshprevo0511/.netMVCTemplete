(function (window) {

    "use strict";

    var tableHelper = {};
    tableHelper.skeleton = function (table, columns, rows) {

        rows = rows || 5;

        var tbody = $(table);

        tbody.empty();

        for (var i = 0; i < rows; i++) {

            var tr = $("<tr>");

            for (var j = 0; j < columns; j++) {

                tr.append(
                    $("<td>").append(
                        $("<div>")
                            .addClass("th-skeleton")
                    )
                );

            }

            tbody.append(tr);

        }

    };
    tableHelper.bind = function (options) {

        var tbody = $(options.table);

        tbody.empty();

        var data = options.data || [];

        if (data.length === 0) {

            tbody.append(
                $("<tr>").append(
                    $("<td>")
                        .attr("colspan", options.columns || 1)
                        .addClass("text-center text-muted")
                        .text(options.emptyMessage || "No records found.")
                )
            );

            return;
        }

        $.each(data, function (index, item) {

            tbody.append(options.row(item, index));

        });

    };
    tableHelper.pagination = function (options) {

        var p = options.pagination || {};

        var currentPage = parseInt(p.CurrentPage || 1);
        var pageSize = parseInt(p.PageSize || 10);
        var totalRecords = parseInt(p.TotalRecords || 0);
        var totalPages = parseInt(p.TotalPages || Math.ceil(totalRecords / pageSize));

        var pagination = $(options.container || "#tablePagination");
        var info = $(options.infoContainer || "#tableInfo");

        pagination.empty();
        info.empty();

        if (totalRecords === 0) {
            info.text("No records found.");
            return;
        }

        info.text(
            "Showing " +
            (((currentPage - 1) * pageSize) + 1) +
            " - " +
            Math.min(currentPage * pageSize, totalRecords) +
            " of " +
            totalRecords +
            " records"
        );

        if (totalPages <= 1)
            return;

        // Previous
        pagination.append(
            $("<li>")
                .addClass("page-item " + (currentPage === 1 ? "disabled" : ""))
                .append(
                    $("<a>")
                        .addClass("page-link")
                        .attr("href", "#")
                        .text("Previous")
                        .on("click", function (e) {
                            e.preventDefault();

                            if (currentPage > 1)
                                options.onPage(currentPage - 1);
                        })
                )
        );

        // Page Numbers
        for (var i = 1; i <= totalPages; i++) {

            (function (page) {

                pagination.append(
                    $("<li>")
                        .addClass("page-item " + (page === currentPage ? "active" : ""))
                        .append(
                            $("<a>")
                                .addClass("page-link")
                                .attr("href", "#")
                                .text(page)
                                .on("click", function (e) {
                                    e.preventDefault();
                                    options.onPage(page);
                                })
                        )
                );

            })(i);
        }

        // Next
        pagination.append(
            $("<li>")
                .addClass("page-item " + (currentPage === totalPages ? "disabled" : ""))
                .append(
                    $("<a>")
                        .addClass("page-link")
                        .attr("href", "#")
                        .text("Next")
                        .on("click", function (e) {
                            e.preventDefault();

                            if (currentPage < totalPages)
                                options.onPage(currentPage + 1);
                        })
                )
        );
    };
    tableHelper.badge = function (text, type) {

        return $("<span>")
            .addClass("badge bg-" + type)
            .text(text);

    };

    tableHelper.statusBadge = function (status) {

        return $("<span>")
            .addClass("badge " + (status ? "bg-success" : "bg-danger"))
            .text(status ? "Active" : "Inactive");

    };

    tableHelper.serialNo = function (pageNumber, pageSize, index) {

        return ((pageNumber - 1) * pageSize) + index + 1;

    };
    tableHelper.error = function (table, columns, message) {

        $(table).html(
            $("<tr>").append(
                $("<td>")
                    .attr("colspan", columns)
                    .addClass("text-center text-danger")
                    .text(message)
            )
        );

    };
    tableHelper.date = function (value) {

        return value ? value.substring(0, 10) : "-";

    };
    tableHelper.actions = {

        edit: function (id) {

            return $("<i>")
                .addClass("bi bi-pencil-square text-primary fs-5 me-2 cursor-pointer")
                .attr("title", "Edit")
                .attr("data-id", id)
                .addClass("btnEdit");

        },

        delete: function (id) {

            return $("<i>")
                .addClass("bi bi-trash text-danger fs-5 me-2 cursor-pointer")
                .attr("title", "Delete")
                .attr("data-id", id)
                .addClass("btnDelete");

        },

        view: function (id) {

            return $("<i>")
                .addClass("bi bi-eye text-success fs-5 me-2 cursor-pointer")
                .attr("title", "View")
                .attr("data-id", id)
                .addClass("btnView");

        },

        download: function (id) {

            return $("<i>")
                .addClass("bi bi-download text-info fs-5 me-2 cursor-pointer")
                .attr("title", "Download")
                .attr("data-id", id)
                .addClass("btnDownload");

        }

    };
    tableHelper.actionCustom = function (options) {

        return $("<i>")
            .addClass("bi " + options.icon)
            .addClass(options.color || "")
            .addClass("fs-5 me-2 cursor-pointer")
            .attr("title", options.title)
            .attr("data-id", options.id)
            .addClass(options.className);

    };
    tableHelper.link = function (text, url) {

        return $("<a>")
            .attr("href", url)
            .addClass("text-decoration-none")
            .text(text);

    };
    tableHelper.copy = function (text) {

        return $("<i>")
            .addClass("bi bi-copy text-secondary fs-5 cursor-pointer")
            .attr("title", "Copy")
            .attr("data-copy", text)
            .on("click", function () {

                navigator.clipboard.writeText(text);

                if (window.showToast)
                    showToast("Copied to clipboard", "success");

            });

    };
    tableHelper.toggle = function (checked, id, className) {

        return $("<div>")
            .addClass("form-check form-switch")
            .append(
                $("<input>")
                    .addClass("form-check-input " + (className || ""))
                    .attr({
                        type: "checkbox",
                        "data-id": id
                    })
                    .prop("checked", checked)
            );

    };
    tableHelper.maskEmail = function (email) {

        if (!email)
            return "";

        var atIndex = email.indexOf("@");

        if (atIndex <= 2)
            return email;

        return email.substring(0, 2)
            + "*".repeat(atIndex - 2)
            + email.substring(atIndex);
    };
    tableHelper.maskMobile = function (mobile) {

        if (!mobile || mobile.length < 10)
            return mobile;

        return mobile.substring(0, 2)
            + "******"
            + mobile.substring(mobile.length - 2);
    };
    tableHelper.maskAadhaar = function (aadhaar) {

        if (!aadhaar || aadhaar.length !== 12)
            return aadhaar;

        return "XXXXXXXX" + aadhaar.substring(8);
    };
    tableHelper.maskPAN = function (pan) {

        if (!pan || pan.length !== 10)
            return pan;

        return pan.substring(0, 3)
            + "*****"
            + pan.substring(8);
    };
    tableHelper.mask = function (value, startVisible, endVisible, maskChar) {

        if (!value)
            return "";

        maskChar = maskChar || "*";

        if (value.length <= (startVisible + endVisible))
            return value;

        return value.substring(0, startVisible)
            + maskChar.repeat(value.length - startVisible - endVisible)
            + value.substring(value.length - endVisible);
    };
    window.tableHelper = tableHelper;

})(window);
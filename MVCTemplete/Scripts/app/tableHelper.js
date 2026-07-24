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
    window.tableHelper = tableHelper;

})(window);
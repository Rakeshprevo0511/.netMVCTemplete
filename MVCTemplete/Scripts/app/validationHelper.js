// validationHelper.js

(function (window) {

    "use strict";

    var validationHelper = {};

    //===========================
    // Required / Null
    //===========================
    validationHelper.required = function (value) {

        return value !== null &&
            value !== undefined &&
            String(value).trim() !== "";

    };

    //===========================
    // Email
    //===========================
    validationHelper.email = function (value) {

        if (!validationHelper.required(value))
            return false;

        return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value);

    };

    //===========================
    // Number
    //===========================
    validationHelper.number = function (value) {

        if (!validationHelper.required(value))
            return false;

        return !isNaN(value);

    };

    //===========================
    // Integer
    //===========================
    validationHelper.integer = function (value) {

        if (!validationHelper.required(value))
            return false;

        return Number.isInteger(Number(value));

    };

    //===========================
    // Mobile (India)
    //===========================
    validationHelper.mobile = function (value) {

        if (!validationHelper.required(value))
            return false;

        return /^[6-9]\d{9}$/.test(value);

    };

    //===========================
    // Password
    // Minimum 8 characters,
    // 1 Upper, 1 Lower,
    // 1 Number, 1 Special
    //===========================
    validationHelper.password = function (value) {

        if (!validationHelper.required(value))
            return false;

        return /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@#$%^&*!]).{8,}$/.test(value);

    };

    //===========================
    // URL
    //===========================
    validationHelper.url = function (value) {

        if (!validationHelper.required(value))
            return false;

        try {
            new URL(value);
            return true;
        }
        catch {
            return false;
        }

    };

    //===========================
    // PAN
    //===========================
    validationHelper.pan = function (value) {

        if (!validationHelper.required(value))
            return false;

        return /^[A-Z]{5}[0-9]{4}[A-Z]{1}$/.test(value.toUpperCase());

    };

    //===========================
    // Aadhaar
    //===========================
    validationHelper.aadhaar = function (value) {

        if (!validationHelper.required(value))
            return false;

        return /^\d{12}$/.test(value);

    };

    //===========================
    // Min Length
    //===========================
    validationHelper.minLength = function (value, length) {

        if (!validationHelper.required(value))
            return false;

        return value.trim().length >= length;

    };

    //===========================
    // Max Length
    //===========================
    validationHelper.maxLength = function (value, length) {

        if (!validationHelper.required(value))
            return false;

        return value.trim().length <= length;

    };

    //===========================
    // Compare
    //===========================
    validationHelper.compare = function (value1, value2) {

        return value1 === value2;

    };

    window.validationHelper = validationHelper;

})(window);
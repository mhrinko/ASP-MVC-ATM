// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


// Jquery Dependency

function goBack() {
    window.history.back();
}

function clearInput(input) {
    input.value = input.value.substring(0, 0);
}

// Data Input Formatting
//------------------------------------------------

// Credit Card Input Formatting
//------------------------------------------------
$("input[data-type='CreditCardNumber']").on({
    keydown: function () {
        //formatCreditCard($(this));
        return false;
    },
    paste: function () {
        //formatCreditCard($(this));
        return false;
    },
    blur: function () {
        //formatCreditCard($(this), "blur");
        return false;
    }
});

function formatCreditCardNumber(n) {
    // format number 1234567 to 123-4567
    return n.replace(/\D/g, "").replace(/\B(?=(\d{4})+(?!\d))/g, "-")
}

function formatCreditCard(input, blur) {
    var input_val = input.val();
    if (input_val === "") { return; }
    var original_len = input_val.length;
    var caret_pos = input.prop("selectionStart");

    input_val = formatCreditCardNumber(input_val);
    input_val = input_val;
    
    input.val(input_val);
    
    var updated_len = input_val.length;
    caret_pos = updated_len - original_len + caret_pos;
    input[0].setSelectionRange(caret_pos, caret_pos);
}

// Pin Input Formatting
//------------------------------------------------
$("input[data-type='Pin']").on({
    keypress: function () {
        //formatPin($(this));
        return false;
    },
    paste: function () {
        //formatPin($(this));
        return false;
    },
    blur: function () {
        //formatPin($(this), "blur");
        return false;
    }
});

function formatPinNumber(n) {
    // format number 1234567 to 123-4567
    return n.replace(/\D/g, "").replace(/\B(?=(\d{4})+(?!\d))/g, "-")
}

function formatPin(input, blur) {
    var input_val = input.val();
    if (input_val === "") { return; }
    
    var original_len = input_val.length;
    var caret_pos = input.prop("selectionStart");

    if (input_val.length > 4) input_val = input_val.substring(0, 4);
    input_val = formatPinNumber(input_val);
    input_val = input_val;
    
    input.val(input_val);
    var updated_len = input_val.length;
    caret_pos = updated_len - original_len + caret_pos;
    input[0].setSelectionRange(caret_pos, caret_pos);
}

// Currency Input Formatting
//------------------------------------------------
$("input[data-type='currency']").on({
    keypress: function () {
        //formatCurrency($(this));
        return false;
    },
    paste: function () {
        //formatCurrency($(this));
        return false;
    },
    blur: function () {
        //formatCurrency($(this), "blur");
        return false;
    }
});

function formatNumber(n) {
    // format number 1000000 to 1,234,567
    return n.replace(/\D/g, "").replace(/\B(?=(\d{3})+(?!\d))/g, "")
}

function formatCurrency(input, blur) {
    // appends $ to value, validates decimal side
    // and puts cursor back in right position.

    // get input value
    var input_val = input.val();

    // don't validate empty input
    if (input_val === "") { return; }

    // original length
    var original_len = input_val.length;

    // initial caret position 
    var caret_pos = input.prop("selectionStart");

    // check for decimal
    if (input_val.indexOf(".") >= 0) {

        // get position of first decimal
        // this prevents multiple decimals from
        // being entered
        var decimal_pos = input_val.indexOf(".");

        // split number by decimal point
        var left_side = input_val.substring(0, decimal_pos);
        var right_side = input_val.substring(decimal_pos);

        // add commas to left side of number
        left_side = formatNumber(left_side);

        // validate right side
        right_side = formatNumber(right_side);

        // On blur make sure 2 numbers after decimal
        if (blur === "blur") {
            right_side += "00";
        }

        // Limit decimal to only 2 digits
        right_side = right_side.substring(0, 2);

        // join number by .
        input_val = left_side + "." + right_side;

    } else {
        // no decimal entered
        // add commas to number
        // remove all non-digits
        input_val = formatNumber(input_val);
        input_val = input_val;

        // final formatting
        if (blur === "blur") {
            input_val += ".00";
        }
    }

    // send updated string to input
    input.val(input_val);

    // put caret back in the right position
    var updated_len = input_val.length;
    caret_pos = updated_len - original_len + caret_pos;
    input[0].setSelectionRange(caret_pos, caret_pos);
}
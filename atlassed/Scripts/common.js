/**
 * POSTs to a webservice and receives a json response.
 *
 * @param {object} options Options for the call
 * Options:
 * {string}     webservice  The name of the webservice to call (do NOT add .webservice file extension)
 * {string}     func        The name of the function to call within the webservice
 * {object}     params      An object containing parameters to pass to the webservice
 * {boolean}    async       Whether the call should be asynchronous or not (exposes underlying async option)
 * {function}   callback    A callback when the call returns.
 *
 * @returns {same as $.ajax()}
 */
function ajax(options) {
    var defaultOptions = {
        webservice: 'site',
        func: undefined,
        async: true,
        params: {},
        success: undefined,
        failure: function (data) {
            alert(data);
        },
        error: function (data) {
            alert(data.statusText + ' ' + data.responseText);
        }
    };

    var internalOptions = $.extend({}, defaultOptions, options);

    if (Object.keys(internalOptions).length > Object.keys(defaultOptions).length)
        throw 'ajax: invalid options';

    if (typeof internalOptions.webservice !== 'string') {
        throw 'ajax: webservice must be a string';
    }
    if (typeof internalOptions.func !== 'string') {
        throw 'ajax: func must be a string';
    }
    if (typeof internalOptions.params !== 'object') {
        throw 'ajax: params must be an object';
    }
    if (typeof internalOptions.async !== 'boolean') {
        throw 'ajax: async must be a boolean';
    }
    if (typeof internalOptions.success !== 'function' && typeof internalOptions.success !== 'undefined') {
        throw 'ajax: success must be a function';
    }
    if (typeof internalOptions.failure !== 'function' && typeof internalOptions.failure !== 'undefined') {
        throw 'ajax: failure must be a function';
    }

    // var a = $.ajax({
    //     url: '' + internalOptions.webservice + 'WebService.asmx/' + internalOptions.func,
    //     data: JSON.stringify(internalOptions.params || {}),
    //     contentType: 'application/json; charset=utf-8',
    //     type: 'post',
    //     dataType: 'json',
    //     async: internalOptions.async,
    //     success: function (data) {
    //         internalOptions.success(data.d);
    //     },
    //     failure: internalOptions.failure,
    //     error: internalOptions.error
    // });
    return null;
}

function CreateTileLink(label, onclick, size) {
    var tile = $('<div/>'),
        inner = tile.clone();
    tile.addClass('tile-link' + (size == '' ? '' : '-' + size));
    tile.click(onclick);

    inner.text(label);
    return tile.append(inner);
}

function centerX(element) {
    return window.innerWidth / 2 - $(element).width() / 2
}

function centerY(element) {
    return window.innerHeight / 2 - $(element).height() / 2
}

function selectOption(value, label) {
    return $('<option value="' + value + '">' + label + '</option>');
}

function tryCall(callback, arg1, arg2, arg3, arg4) {
    if (typeof callback === 'function') {
        callback.call(null, arg1, arg2, arg3, arg4);
    }
}

function rectGetCentre(topLeft, bottomRight) {
    return {
        X: topLeft.X + (bottomRight.X - topLeft.X) / 2,
        Y: topLeft.Y + (bottomRight.Y - topLeft.Y) / 2
    }
}
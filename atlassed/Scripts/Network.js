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
        async: true,
        params: {},
        success: undefined,
        type: 'get',
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
    if (internalOptions.type !== 'get' && internalOptions.type !== 'post' && 
        internalOptions.type !== 'put' && internalOptions.type !== 'delete') {
        throw 'ajax: type is not valid';
    }

    if (internalOptions.type === 'get') {
        internalOptions.params = $.param( internalOptions.params );
    }
    else if (internalOptions.type === 'post' || internalOptions.type === 'put') {
        internalOptions.params.metaproperties = 
            JSON.stringify( internalOptions.params.metaproperties || {});
        internalOptions.params.metaproperties = internalOptions.params.metaproperties.replace (/"/g,'');;
    }

    var a = $.ajax({
        url: internalOptions.webservice,
        data: internalOptions.params,
        type: internalOptions.type,
        async: internalOptions.async,
        success: function (data) {
            internalOptions.success(data);
        },
        failure: internalOptions.failure,
        error: internalOptions.error
    });

    return a;
}


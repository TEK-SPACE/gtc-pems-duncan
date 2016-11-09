
/*
Changes all dataSources to case insensitive sorting (client side sorting).
This snipped enable case insensitive sorting on Kendo UI grid, too.

The original case sensitive comparer is a private and can't be accessed without modifying the original source code.
tested with Kendo UI version 2012.2.710 (Q2 2012 / July 2012).
*/

var CaseInsensitiveComparer = {

    getterCache: {},

    getter: function (expression) {
        return this.getterCache[expression] = this.getterCache[expression] || new Function("d", "return " + kendo.expr(expression));
    },

    selector: function (field) {
        return jQuery.isFunction(field) ? field : this.getter(field);
    },

    asc: function (field) {
        var selector = this.selector(field);
        return function (a, b) {

            a = selector(a);
            b = selector(b);

            if (typeof a == "string" && typeof b == "string") {
                a = a.toLowerCase();
                b = b.toLowerCase();
            }

            return a > b ? 1 : (a < b ? -1 : 0);
        };
    },

    desc: function (field) {
        var selector = this.selector(field);
        return function (a, b) {

            a = selector(a);
            b = selector(b);

            if (typeof a == "string" && typeof b == "string") {
                a = a.toLowerCase();
                b = b.toLowerCase();
            }

            return a < b ? 1 : (a > b ? -1 : 0);
        };
    },

    create: function (descriptor) {
        return this[descriptor.dir.toLowerCase()](descriptor.field);
    },

    combine: function (comparers) {
        return function (a, b) {
            var result = comparers[0](a, b),
                idx,
                length;

            for (idx = 1, length = comparers.length; idx < length; idx++) {
                result = result || comparers[idx](a, b);
            }

            return result;
        };
    }
};

kendo.data.Query.prototype.normalizeSort = function (field, dir) {
    if (field) {
        var descriptor = typeof field === "string" ? { field: field, dir: dir } : field,
            descriptors = jQuery.isArray(descriptor) ? descriptor : (descriptor !== undefined ? [descriptor] : []);

        return jQuery.grep(descriptors, function (d) { return !!d.dir; });
    }
};

kendo.data.Query.prototype.sort = function (field, dir, comparer) {

    var idx,
        length,
        descriptors = this.normalizeSort(field, dir),
        comparers = [];

    comparer = comparer || CaseInsensitiveComparer;

    if (descriptors.length) {
        for (idx = 0, length = descriptors.length; idx < length; idx++) {
            comparers.push(comparer.create(descriptors[idx]));
        }

        return this.orderBy({ compare: comparer.combine(comparers) });
    }

    return this;
};

kendo.data.Query.prototype.orderBy = function (selector) {

    var result = this.data.slice(0),
        comparer = jQuery.isFunction(selector) || !selector ? CaseInsensitiveComparer.asc(selector) : selector.compare;

    return new kendo.data.Query(result.sort(comparer));
};

kendo.data.Query.prototype.orderByDescending = function (selector) {

    return new kendo.data.Query(this.data.slice(0).sort(CaseInsensitiveComparer.desc(selector)));
};


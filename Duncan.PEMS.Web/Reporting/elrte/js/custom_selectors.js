function initFields(selectControl) {
    if (selectControl == undefined || selectControl == null)
        return null;

    var getColumnName = function (fullName) {
        var column = fullName;
        var groups = fullName.split('.');
        if (groups.length == 3)
            column = groups[2];
        return column;
    };

    var fields = {};
    fields[''] = 'Field';

    select$ = $(selectControl);
    if (select$.find('optgroup').length == 0) {
        select$.find('option').each(function () {
            if ($(this).val() != '...')
                fields[getColumnName($(this).val())] = $(this).text();
        });
    }
    else {
        var allFields = [];
        $(selectControl).find('optgroup').each(function () {
            $(this).find('option').each(function () {
                allFields.push($(this).val());
            });
        });
        var fieldsWithDups = [];
        for (var i = 0; i < allFields.length; i++) {
            for (var k = 0; k < allFields.length; k++)
                if (i != k && getColumnName(allFields[k]) == getColumnName(allFields[i])) {
                    fieldsWithDups.push(allFields[i]);
                    break;
                }
        }

        $(selectControl).find('optgroup').each(function () {
            fields['tbl- ' + $(this).attr('label')] = '<b>' + $(this).attr('label') + '</b>';
            $(this).find('option').each(function () {
                var fieldVal = $(this).val();
                if ($.inArray(fieldVal, fieldsWithDups) == -1)
                    fieldVal = getColumnName(fieldVal);
                fields[fieldVal] = '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;' + $(this).text();
            });
        });
    }
    fieldsDropDown.reinit(fields);

}


function initSubreports(selectControl) {
    if (selectControl == undefined || selectControl == null)
        return null;

    var subreports = {};
    subreports[''] = 'Subreport';

    select$ = $(selectControl);
    select$.find('option').each(function () {
        if ($(this).val() != '...' && $(this).val() != '' && $(this).val() != '(AUTO)')
            subreports['[[' + $(this).val() + ']]'] = $(this).text();
    });

    subreportsDropDown.reinit(subreports);
}

var fieldsDropDown;
var subreportsDropDown;
(function ($) {
    elRTE.prototype.ui.prototype.buttons.field = function (rte, name) {
        this.constructor.prototype.constructor.call(this, rte, name);
        var self = this;
        var opts = {
            tpl: '<span value="%val">%label</span>',
            select: function (v) { self.set(v); },
            width: '400px',
            src: {}
        }
        self.opts = opts;

        this.select = this.domElem.elSelect(opts);
        fieldsDropDown = this;
        this.command = function () {
        }

        this.reinit = function (fields) {
            var opts = self.opts;
            opts.src = fields;
            self.domElem.elSelect(opts);
        }

        this.set = function (val) {
            this.rte.history.add();

            if (val.toString().indexOf('tbl') != 0)
                this.rte.selection.insertHtml(val.toString());

            this.rte.ui.update();
            //this.val('');
        }

        this.update = function () {
            this.domElem.removeClass('disabled');
            var n = this.rte.selection.getNode();
            if (n.nodeType != 1) {
                n = n.parentNode;
            }
            var v = $(n).css('font-family');
            v = v ? v.toString().toLowerCase().replace(/,\s+/g, ',').replace(/'|"/g, '') : '';
            this.select.val(opts.src[v] ? v : '');
        }
    };

    elRTE.prototype.ui.prototype.buttons.subreports = function (rte, name) {
        this.constructor.prototype.constructor.call(this, rte, name);
        var self = this;
        var opts = {
            tpl: '<span value="%val" title="%label">%label</span>',
            select: function (v) { self.set(v); },
            width: '400px',
            src: {}
        }
        self.opts = opts;

        this.select = this.domElem.elSelect(opts);
        subreportsDropDown = this;
        this.command = function () {
        }

        this.reinit = function (subreports) {
            var opts = self.opts;
            opts.src = subreports;
            self.domElem.elSelect(opts);
        }

        this.set = function (val) {
            this.rte.history.add();
            this.rte.selection.insertHtml(val.toString());
            this.rte.ui.update();
            //this.val('');
        }

        this.update = function () {
            this.domElem.removeClass('disabled');
            var n = this.rte.selection.getNode();
            if (n.nodeType != 1) {
                n = n.parentNode;
            }
            var v = $(n).css('font-family');
            v = v ? v.toString().toLowerCase().replace(/,\s+/g, ',').replace(/'|"/g, '') : '';
            this.select.val(opts.src[v] ? v : '');
        }
    };

    elRTE.prototype.ui.prototype.buttons.tags = function (rte, name) {
        this.constructor.prototype.constructor.call(this, rte, name);
        var self = this;
        var opts = {
            tpl: '<span value="%val" title="%val">%label</span>',
            select: function (v) { self.set(v); },
            width: '400px',
            src: {
                '': 'Tags',
                ' ': ' ',
                '[]': 'Field',
                '[@Subtotal]': 'Field Subtotal',
                '[[]]': 'Subreport',
                '[REPEATER][/REPEATER]': 'Repeater',
                '<!--VISUALISATION--><!--VISUALISATION-->': 'Raw Content',
                '[Date]': 'Date',
                '[Filters]': 'Filters'
            }
        }
        self.opts = opts;

        this.select = this.domElem.elSelect(opts);
        subreportsDropDown = this;
        this.command = function () {
        }

        this.set = function (val) {
            this.rte.history.add();
            if (val != '' && val != ' ')
                this.rte.selection.insertHtml(val.toString());
            this.rte.ui.update();
            //this.val('');
        }

        this.update = function () {
            this.domElem.removeClass('disabled');            
            var n = this.rte.selection.getNode();
            if (n) {
              if (n.nodeType != 1) {
                  n = n.parentNode;
              }
              var v = $(n).css('font-family');
              v = v ? v.toString().toLowerCase().replace(/,\s+/g, ',').replace(/'|"/g, '') : '';
              this.select.val(opts.src[v] ? v : '');
            }
        }
    };
})(jQuery);
﻿(function (e) { "function" === typeof define && define.amd ? define(["jquery", "datatables.net"], function (i) { return e(i, window, document) }) : "object" === typeof exports ? module.exports = function (i, l) { i || (i = window); if (!l || !l.fn.dataTable) l = require("datatables.net")(i, l).$; return e(l, i, i.document) } : e(jQuery, window, document) })(function (e, i, l, h) {
    function t(b, a, c) {
        var d; d = function (c, a) { if (c > a) var d = a, a = c, c = d; var f = !1; return b.columns(":visible").indexes().filter(function (b) { b === c && (f = !0); return b === a ? (f = !1, !0) : f }) }; var f =
        function (c, a) { var d = b.rows({ search: "applied" }).indexes(); if (d.indexOf(c) > d.indexOf(a)) var f = a, a = c, c = f; var e = !1; return d.filter(function (b) { b === c && (e = !0); return b === a ? (e = !1, !0) : e }) }; !b.cells({ selected: !0 }).any() && !c ? (d = d(0, a.column), c = f(0, a.row)) : (d = d(c.column, a.column), c = f(c.row, a.row)); c = b.cells(c, d).flatten(); b.cells(a, { selected: !0 }).any() ? b.cells(c).deselect() : b.cells(c).select()
    } function r(b) {
        var a = b.settings()[0]._select.selector; e(b.table().body()).off("mousedown.dtSelect", a).off("mouseup.dtSelect",
        a).off("click.dtSelect", a); e("body").off("click.dtSelect")
    } function v(b) {
        var a = e(b.table().body()), c = b.settings()[0], d = c._select.selector; a.on("mousedown.dtSelect", d, function (c) { if (c.shiftKey || c.metaKey || c.ctrlKey) a.css("-moz-user-select", "none").one("selectstart.dtSelect", d, function () { return !1 }) }).on("mouseup.dtSelect", d, function () { a.css("-moz-user-select", "") }).on("click.dtSelect", d, function (c) {
            var a = b.select.items(); if (!i.getSelection || !i.getSelection().toString()) {
                var d = b.settings()[0]; if (e(c.target).closest("div.dataTables_wrapper")[0] ==
                b.table().container()) { var g = b.cell(e(c.target).closest("td, th")); if (g.any()) { var h = e.Event("user-select.dt"); k(b, h, [a, g, c]); h.isDefaultPrevented() || (h = g.index(), "row" === a ? (a = h.row, s(c, b, d, "row", a)) : "column" === a ? (a = g.index().column, s(c, b, d, "column", a)) : "cell" === a && (a = g.index(), s(c, b, d, "cell", a)), d._select_lastCell = h) } }
            }
        }); e("body").on("click.dtSelect", function (a) { c._select.blurable && !e(a.target).parents().filter(b.table().container()).length && (e(a.target).parents("div.DTE").length || p(c, !0)) })
    } function k(b,
    a, c, d) { if (!d || b.flatten().length) "string" === typeof a && (a += ".dt"), c.unshift(b), e(b.table().node()).triggerHandler(a, c) } function w(b) {
        var a = b.settings()[0]; if (a._select.info && a.aanFeatures.i) {
            var c = e('<span class="select-info"/>'), d = function (a, d) { c.append(e('<span class="select-item"/>').append(b.i18n("select." + a + "s", { _: "%d " + a + "s selected", "0": "", 1: "1 " + a + " selected" }, d))) }; d("row", b.rows({ selected: !0 }).flatten().length); d("column", b.columns({ selected: !0 }).flatten().length); d("cell", b.cells({ selected: !0 }).flatten().length);
            e.each(a.aanFeatures.i, function (a, b) { var b = e(b), d = b.children("span.select-info"); d.length && d.remove(); "" !== c.text() && b.append(c) })
        }
    } function x(b, a, c, d) { var f = b[a + "s"]({ search: "applied" }).indexes(), d = e.inArray(d, f), m = e.inArray(c, f); if (!b[a + "s"]({ selected: !0 }).any() && -1 === d) f.splice(e.inArray(c, f) + 1, f.length); else { if (d > m) var j = m, m = d, d = j; f.splice(m + 1, f.length); f.splice(0, d) } b[a](c, { selected: !0 }).any() ? (f.splice(e.inArray(c, f), 1), b[a + "s"](f).deselect()) : b[a + "s"](f).select() } function p(b, a) {
        if (a || "single" ===
        b._select.style) { var c = new g.Api(b); c.rows({ selected: !0 }).deselect(); c.columns({ selected: !0 }).deselect(); c.cells({ selected: !0 }).deselect() }
    } function s(b, a, c, d, f) {
        var e = a.select.style(), j = a[d](f, { selected: !0 }).any(); "os" === e ? b.ctrlKey || b.metaKey ? a[d](f).select(!j) : b.shiftKey ? "cell" === d ? t(a, f, c._select_lastCell || null) : x(a, d, f, c._select_lastCell ? c._select_lastCell[d] : null) : (b = a[d + "s"]({ selected: !0 }), j && 1 === b.flatten().length ? a[d](f).deselect() : (b.deselect(), a[d](f).select())) : "multi+shift" == e ? b.shiftKey ?
        "cell" === d ? t(a, f, c._select_lastCell || null) : x(a, d, f, c._select_lastCell ? c._select_lastCell[d] : null) : a[d](f).select(!j) : a[d](f).select(!j)
    } function q(b, a) { return function (c) { return c.i18n("buttons." + b, a) } } var g = e.fn.dataTable; g.select = {}; g.select.version = "1.2.0"; g.select.init = function (b) {
        var a = b.settings()[0], c = a.oInit.select, d = g.defaults.select, c = c === h ? d : c, d = "row", f = "api", m = !1, j = !0, u = "td, th", i = "selected"; a._select = {}; if (!0 === c) f = "os"; else if ("string" === typeof c) f = c; else if (e.isPlainObject(c) && (c.blurable !==
        h && (m = c.blurable), c.info !== h && (j = c.info), c.items !== h && (d = c.items), c.style !== h && (f = c.style), c.selector !== h && (u = c.selector), c.className !== h)) i = c.className; b.select.selector(u); b.select.items(d); b.select.style(f); b.select.blurable(m); b.select.info(j); a._select.className = i; e.fn.dataTable.ext.order["select-checkbox"] = function (a, c) {
            return this.api().column(c, { order: "index" }).nodes().map(function (c) {
                return "row" === a._select.items ? e(c).parent().hasClass(a._select.className) : "cell" === a._select.items ? e(c).hasClass(a._select.className) :
                !1
            })
        }; e(b.table().node()).hasClass("selectable") && b.select.style("os")
    }; e.each([{ type: "row", prop: "aoData" }, { type: "column", prop: "aoColumns" }], function (b, a) { g.ext.selector[a.type].push(function (c, b, f) { var b = b.selected, e, j = []; if (b === h) return f; for (var g = 0, i = f.length; g < i; g++) e = c[a.prop][f[g]], (!0 === b && !0 === e._select_selected || !1 === b && !e._select_selected) && j.push(f[g]); return j }) }); g.ext.selector.cell.push(function (b, a, c) {
        var a = a.selected, d, f = []; if (a === h) return c; for (var e = 0, g = c.length; e < g; e++) d = b.aoData[c[e].row],
        (!0 === a && d._selected_cells && !0 === d._selected_cells[c[e].column] || !1 === a && (!d._selected_cells || !d._selected_cells[c[e].column])) && f.push(c[e]); return f
    }); var n = g.Api.register, o = g.Api.registerPlural; n("select()", function () { return this.iterator("table", function (b) { g.select.init(new g.Api(b)) }) }); n("select.blurable()", function (b) { return b === h ? this.context[0]._select.blurable : this.iterator("table", function (a) { a._select.blurable = b }) }); n("select.info()", function (b) {
        return w === h ? this.context[0]._select.info :
        this.iterator("table", function (a) { a._select.info = b })
    }); n("select.items()", function (b) { return b === h ? this.context[0]._select.items : this.iterator("table", function (a) { a._select.items = b; k(new g.Api(a), "selectItems", [b]) }) }); n("select.style()", function (b) {
        return b === h ? this.context[0]._select.style : this.iterator("table", function (a) {
            a._select.style = b; if (!a._select_init) {
                var c = new g.Api(a); a.aoRowCreatedCallback.push({
                    fn: function (c, b, d) {
                        b = a.aoData[d]; b._select_selected && e(c).addClass(a._select.className);
                        c = 0; for (d = a.aoColumns.length; c < d; c++) (a.aoColumns[c]._select_selected || b._selected_cells && b._selected_cells[c]) && e(b.anCells[c]).addClass(a._select.className)
                    }, sName: "select-deferRender"
                }); c.on("preXhr.dt.dtSelect", function () {
                    var a = c.rows({ selected: !0 }).ids(!0).filter(function (c) { return c !== h }), b = c.cells({ selected: !0 }).eq(0).map(function (a) { var b = c.row(a.row).id(!0); return b ? { row: b, column: a.column } : h }).filter(function (c) { return c !== h }); c.one("draw.dt.dtSelect", function () {
                        c.rows(a).select(); b.any() &&
                        b.each(function (a) { c.cells(a.row, a.column).select() })
                    })
                }); c.on("draw.dtSelect.dt select.dtSelect.dt deselect.dtSelect.dt info.dt", function () { w(c) }); c.on("destroy.dtSelect", function () { r(c); c.off(".dtSelect") })
            } var d = new g.Api(a); r(d); "api" !== b && v(d); k(new g.Api(a), "selectStyle", [b])
        })
    }); n("select.selector()", function (b) { return b === h ? this.context[0]._select.selector : this.iterator("table", function (a) { r(new g.Api(a)); a._select.selector = b; "api" !== a._select.style && v(new g.Api(a)) }) }); o("rows().select()",
    "row().select()", function (b) { var a = this; if (!1 === b) return this.deselect(); this.iterator("row", function (c, a) { p(c); c.aoData[a]._select_selected = !0; e(c.aoData[a].nTr).addClass(c._select.className) }); this.iterator("table", function (c, b) { k(a, "select", ["row", a[b]], !0) }); return this }); o("columns().select()", "column().select()", function (b) {
        var a = this; if (!1 === b) return this.deselect(); this.iterator("column", function (a, b) {
            p(a); a.aoColumns[b]._select_selected = !0; var f = (new g.Api(a)).column(b); e(f.header()).addClass(a._select.className);
            e(f.footer()).addClass(a._select.className); f.nodes().to$().addClass(a._select.className)
        }); this.iterator("table", function (c, b) { k(a, "select", ["column", a[b]], !0) }); return this
    }); o("cells().select()", "cell().select()", function (b) {
        var a = this; if (!1 === b) return this.deselect(); this.iterator("cell", function (a, b, f) { p(a); b = a.aoData[b]; b._selected_cells === h && (b._selected_cells = []); b._selected_cells[f] = !0; b.anCells && e(b.anCells[f]).addClass(a._select.className) }); this.iterator("table", function (b, d) {
            k(a, "select",
            ["cell", a[d]], !0)
        }); return this
    }); o("rows().deselect()", "row().deselect()", function () { var b = this; this.iterator("row", function (a, b) { a.aoData[b]._select_selected = !1; e(a.aoData[b].nTr).removeClass(a._select.className) }); this.iterator("table", function (a, c) { k(b, "deselect", ["row", b[c]], !0) }); return this }); o("columns().deselect()", "column().deselect()", function () {
        var b = this; this.iterator("column", function (a, b) {
            a.aoColumns[b]._select_selected = !1; var d = new g.Api(a), f = d.column(b); e(f.header()).removeClass(a._select.className);
            e(f.footer()).removeClass(a._select.className); d.cells(null, b).indexes().each(function (b) { var c = a.aoData[b.row], d = c._selected_cells; c.anCells && (!d || !d[b.column]) && e(c.anCells[b.column]).removeClass(a._select.className) })
        }); this.iterator("table", function (a, c) { k(b, "deselect", ["column", b[c]], !0) }); return this
    }); o("cells().deselect()", "cell().deselect()", function () {
        var b = this; this.iterator("cell", function (a, b, d) { b = a.aoData[b]; b._selected_cells[d] = !1; b.anCells && !a.aoColumns[d]._select_selected && e(b.anCells[d]).removeClass(a._select.className) });
        this.iterator("table", function (a, c) { k(b, "deselect", ["cell", b[c]], !0) }); return this
    }); e.extend(g.ext.buttons, {
        selected: { text: q("selected", "Selected"), className: "buttons-selected", init: function (b) { var a = this; b.on("draw.dt.DT select.dt.DT deselect.dt.DT", function () { var b = a.rows({ selected: !0 }).any() || a.columns({ selected: !0 }).any() || a.cells({ selected: !0 }).any(); a.enable(b) }); this.disable() } }, selectedSingle: {
            text: q("selectedSingle", "Selected single"), className: "buttons-selected-single", init: function (b) {
                var a =
                this; b.on("draw.dt.DT select.dt.DT deselect.dt.DT", function () { var c = b.rows({ selected: !0 }).flatten().length + b.columns({ selected: !0 }).flatten().length + b.cells({ selected: !0 }).flatten().length; a.enable(1 === c) }); this.disable()
            }
        }, selectAll: { text: q("selectAll", "Select all"), className: "buttons-select-all", action: function () { this[this.select.items() + "s"]().select() } }, selectNone: {
            text: q("selectNone", "Deselect all"), className: "buttons-select-none", action: function () { p(this.settings()[0], !0) }, init: function (b) {
                var a =
                this; b.on("draw.dt.DT select.dt.DT deselect.dt.DT", function () { var c = b.rows({ selected: !0 }).flatten().length + b.columns({ selected: !0 }).flatten().length + b.cells({ selected: !0 }).flatten().length; a.enable(0 < c) }); this.disable()
            }
        }
    }); e.each(["Row", "Column", "Cell"], function (b, a) {
        var c = a.toLowerCase(); g.ext.buttons["select" + a + "s"] = {
            text: q("select" + a + "s", "Select " + c + "s"), className: "buttons-select-" + c + "s", action: function () { this.select.items(c) }, init: function (a) {
                var b = this; a.on("selectItems.dt.DT", function (a, d,
                e) { b.active(e === c) })
            }
        }
    }); e(l).on("preInit.dt.dtSelect", function (b, a) { "dt" === b.namespace && g.select.init(new g.Api(a)) }); return g.select
});
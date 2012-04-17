﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wysnan.EIMOnline.Common.Framework.Grid;
using Wysnan.EIMOnline.Common.Framework.Enum;
using System.Web;
using Wysnan.EIMOnline.Tool.Extensions;
using Wysnan.EIMOnline.Common.Framework;
using Wysnan.EIMOnline.Common.Poco;
using Wysnan.EIMOnline.Tool.ToolMethod;

namespace Wysnan.EIMOnline.Tool.JqGridExtansions
{
    public static class JqGridHelp
    {
        public static string ConvertToHtml(this JqGrid jqGrid)
        {
            if (jqGrid == null)
            {
                return null;
            }
            var colModel = jqGrid._ColModel;
            if (colModel == null || colModel.Count == 0)
            {
                return null;
            }
            string url = HttpContext.Current.Request.Url.AbsolutePath;
            string listUrl = url + "/List";
            string setJqGridColumn = url + "/SetJqGridColumn";
            StringBuilder grid = new StringBuilder();
            StringBuilder StrColNames = new StringBuilder();
            StringBuilder StrColModel = new StringBuilder();
            foreach (var item in colModel)
            {
                StrColNames.AppendFormat("'{0}',", item.Label);
                StrColModel.AppendFormat("{0} name: '{1}', index: '{2}', width: {3}, editable: {4},align: '{5}',datefmt:'{6}'{7},searchoptions:{8},hidden:{9}{10},",
                    "{", item.Name, item.Name, item.Width, item.Editable.ConvertToString(true), item.Align, item.Detefmt, item.Formatter,
                    item.SearchOptions.ToString(), item.Hidden.ConvertToString(true), "}");
            }
            StrColNames.Remove(StrColNames.Length - 1, 1);
            StrColModel.Remove(StrColModel.Length - 1, 1);
            grid.Append("<table id=\"list\" style=\"width:100%;\" ");
            grid.Append("</table>");
            grid.Append("<div id=\"pager\"");
            grid.Append("</div>");
            grid.Append("<script type=\"text/javascript\">");
            grid.Append("$(document).ready(function () {");
            grid.Append("$(\"#list\").jqGrid({");
            grid.AppendFormat("url: '{0}',", listUrl);// jqGrid._Url);
            grid.Append("autowidth:true,");
            //grid.Append("setGridWidth:100%,");
            grid.AppendFormat("datatype: '{0}',", jqGrid._DataType);
            grid.AppendFormat("mtype: '{0}',", jqGrid._Mtype);
            grid.AppendFormat("colNames: [{0}],", StrColNames.ToString());
            grid.Append("colModel: [");
            grid.Append(StrColModel.ToString());
            grid.Append("],");
            grid.Append("pager: '#pager',");
            grid.AppendFormat("sortable: {0},", jqGrid._Sortable.ConvertToString(true));
            grid.AppendFormat("rowNum: {0},", jqGrid.RowNum);
            grid.Append("multiselect: true,");
            grid.Append("prmNames: { page:\"page\",rows:\"rows\", sort: \"sidx\",order:\"sord\", search:\"search\", nd:\"nd\", npage:\"npage\" },");
            grid.Append("jsonReader: {");
            grid.AppendFormat("root: \"{0}\",", "rows");// jqGrid._JsonReader_Root);
            grid.AppendFormat("repeatitems: {0}", jqGrid._JsonReader_Repeatitems.ConvertToString(true));
            grid.Append("},");
            grid.AppendFormat("rowList: [{0}],", jqGrid._RowList.ConvertToString(","));
            grid.AppendFormat("sortname: '{0}',", jqGrid.SortName);
            grid.AppendFormat("sortorder: '{0}',", jqGrid.SortOrder);
            grid.AppendFormat("height:{0},", 250);
            grid.AppendFormat("viewrecords: {0},", jqGrid._ViewRecords.ConvertToString(true));
            grid.AppendFormat("caption: '&nbsp;{0}'", jqGrid._Caption);
            grid.Append("});");
            grid.Append("var grid=$(\"#list\");");
            grid.Append("grid.jqGrid('navGrid', '#pager', { edit: false, add: true, del: true,view:false },{},{},{},{multipleSearch:true,overlay:false,closeAfterSearch:true,closeOnEscape:true});");
            grid.Append("grid.jqGrid('filterToolbar',{stringResult: true,searchOnEnter : true});");
            //grid.Append("grid.jqGrid('navButtonAdd','#pager',{caption: \"\",buttonicon: \"ui-icon-calculator\", title:\"Reorder Columns\",onClickButton:function(){grid.jqGrid('columnChooser');}});");
            grid.Append("grid.jqGrid('navButtonAdd','#pager',{caption: \"\",buttonicon: \"ui-icon-calculator\", title:\"Reorder Columns\",onClickButton:function(){grid.jqGrid('columnChooser',{ 'done': function(perm) { if (perm) { this.jqGrid('remapColumns', perm, true); persist(this);}}});}});");
            grid.Append("});");
            grid.AppendFormat("var SetJqGridColumn='{0}';", setJqGridColumn);
            grid.Append("</script>");
            string gridHtml = grid.ToString();
            return gridHtml;
        }

        public static void WriteCookie(this JqGrid jqGrid, GridEnum gridEnum)
        {
            string gridHtml = jqGrid.ConvertToHtml();
            jqGrid.WriteCookie(gridEnum, gridHtml);
        }
        public static void WriteCookie(this JqGrid jqGrid, GridEnum gridEnum, string gridHtml)
        {
            string key = SystemEntity.Instance.CurrentSecurityUser.ID + "_" + gridEnum.ToString();
            HttpCookie cookie = HttpContext.Current.Request.Cookies[ConstEntity.Cookie_JqGridHtml];
            string url = HttpContext.Current.Request.Url.AbsolutePath + "/List";
            if (cookie != null && cookie.HasKeys)
            {
                //cookie不为空
                var value = cookie.Values.AllKeys.Where(a => a == key).FirstOrDefault();
                if (string.IsNullOrEmpty(value))
                {
                    //添加新cookie key
                    Dictionary<string, string> dic = new Dictionary<string, string>();//原cookie中dic
                    foreach (var item in cookie.Values.AllKeys)
                    {
                        dic[item] = cookie.Values[item];
                    }
                    dic[key] = HttpUtility.UrlEncode(gridHtml);
                    CookieHelp.WriteCookie(ConstEntity.Cookie_JqGridHtml, dic, -1, url);
                }
                else
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>();//原cookie中dic
                    foreach (var item in cookie.Values.AllKeys)
                    {
                        if (item != key)
                        {
                            dic[item] = cookie.Values[item];
                        }
                        else
                        {
                            dic[item] = HttpUtility.UrlEncode(gridHtml);
                        }
                    }
                    CookieHelp.WriteCookie(ConstEntity.Cookie_JqGridHtml, dic, -1, url);
                }

            }
            else
            {
                //cookie为空，添加cookie
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic[key] = HttpUtility.UrlEncode(gridHtml);
                CookieHelp.WriteCookie(ConstEntity.Cookie_JqGridHtml, dic, -1, url);
            }
        }
    }
}
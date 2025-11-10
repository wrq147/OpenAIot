using Common.Share;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using TemplateAction.Common;
using TemplateAction.Core;
namespace Common
{
    public class ExcelHelper
    {
        private ITAServiceProvider _provider;
        public ExcelHelper(ITAServiceProvider provider)
        {
            _provider = provider;
        }

        public byte[] ExportToBuffer(string SheetName, DataTable dt)
        {
            HSSFWorkbook wb = new HSSFWorkbook();
            HSSFSheet sheet = (HSSFSheet)wb.CreateSheet(SheetName); //创建工作表
            sheet.CreateFreezePane(0, 1); //冻结列头行
            HSSFRow row_Title = (HSSFRow)sheet.CreateRow(0); //创建列头行
            row_Title.HeightInPoints = 30.5F; //设置列头行高
            HSSFCellStyle cs_Title = (HSSFCellStyle)wb.CreateCellStyle(); //创建列头样式
            cs_Title.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center; //水平居中
            cs_Title.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center; //垂直居中
            HSSFFont cs_Title_Font = (HSSFFont)wb.CreateFont(); //创建字体
            cs_Title_Font.IsBold = true; //字体加粗
            cs_Title_Font.FontHeightInPoints = 14; //字体大小
            cs_Title.SetFont(cs_Title_Font); //将字体绑定到样式
            #region 生成列头
            int ii = 0;
            foreach (DataColumn col in dt.Columns)
            {
                HSSFCell cell_Title = (HSSFCell)row_Title.CreateCell(ii); //创建单元格
                cell_Title.CellStyle = cs_Title; //将样式绑定到单元格
                cell_Title.SetCellValue(col.ColumnName);
                sheet.SetColumnWidth(ii, 25 * 256);//设置列宽
                ii++;
            }

            #endregion


            HSSFCellStyle cs_Content = (HSSFCellStyle)wb.CreateCellStyle(); //创建列头样式
            cs_Content.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center; //水平居中
            cs_Content.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center; //垂直居中
            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                HSSFRow row_Content = (HSSFRow)sheet.CreateRow(i + 1); //创建行
                row_Content.HeightInPoints = 20;
                int jj = 0;
                foreach (DataColumn col in dt.Columns)
                {
                    HSSFCell cell_Conent = (HSSFCell)row_Content.CreateCell(jj); //创建单元格
                    cell_Conent.CellStyle = cs_Content;

                    object value = dr[col];
                    string cell_value = value == null ? "" : value.ToString();
                    if (cell_value.StartsWith("@Image"))
                    {
                        AddCellPicture(sheet, wb, cell_value.Substring(6), cell_Conent.Address.Row, cell_Conent.Address.Row, cell_Conent.Address.Column, cell_Conent.Address.Column);
                    }
                    else
                    {
                        cell_Conent.SetCellValue(cell_value);
                    }

                    jj++;
                }
                i++;
            }
            MemoryStream stream = new MemoryStream();
            wb.Write(stream);
            return stream.ToArray();
        }
        /// <summary>
        /// 导出通用excel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="SheetName"></param>
        /// <param name="list"></param>
        /// <param name="FiedNames">列头</param>
        /// <returns></returns>
        public byte[] ExportToBuffer<T>(string SheetName, List<T> list, Dictionary<string, ParamRenderToExcel<T>> FiedNames)
        {
            HSSFWorkbook wb = new HSSFWorkbook();
            HSSFSheet sheet = (HSSFSheet)wb.CreateSheet(SheetName); //创建工作表
            sheet.CreateFreezePane(0, 1); //冻结列头行
            HSSFRow row_Title = (HSSFRow)sheet.CreateRow(0); //创建列头行
            row_Title.HeightInPoints = 30.5F; //设置列头行高
            HSSFCellStyle cs_Title = (HSSFCellStyle)wb.CreateCellStyle(); //创建列头样式
            cs_Title.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center; //水平居中
            cs_Title.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center; //垂直居中
            HSSFFont cs_Title_Font = (HSSFFont)wb.CreateFont(); //创建字体
            cs_Title_Font.IsBold = true; //字体加粗
            cs_Title_Font.FontHeightInPoints = 14; //字体大小
            cs_Title.SetFont(cs_Title_Font); //将字体绑定到样式

            #region 生成列头
            int ii = 0;
            foreach (var kvp in FiedNames)
            {
                HSSFCell cell_Title = (HSSFCell)row_Title.CreateCell(ii); //创建单元格
                cell_Title.CellStyle = cs_Title; //将样式绑定到单元格
                cell_Title.SetCellValue(kvp.Value.Name);
                sheet.SetColumnWidth(ii, 25 * 256);//设置列宽
                ii++;
            }
            #endregion

            // 获得此模型的公共属性
            var propertys = typeof(T).GetProperties();
            Dictionary<string, PropertyInfo> dictProps = new Dictionary<string, PropertyInfo>();
            foreach (PropertyInfo property in propertys)
            {
                dictProps.Add(property.Name, property);
            }

            HSSFCellStyle cs_Content = (HSSFCellStyle)wb.CreateCellStyle(); //创建列头样式
            cs_Content.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center; //水平居中
            cs_Content.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center; //垂直居中
            for (int i = 0; i < list.Count; i++)
            {

                HSSFRow row_Content = (HSSFRow)sheet.CreateRow(i + 1); //创建行
                row_Content.HeightInPoints = 20;
                int jj = 0;
                foreach (var kvp in FiedNames)
                {
                    PropertyInfo outProp;
                    if (dictProps.TryGetValue(kvp.Key, out outProp))
                    {
                        HSSFCell cell_Conent = (HSSFCell)row_Content.CreateCell(jj); //创建单元格
                        cell_Conent.CellStyle = cs_Content;
                        object val = null;
                        if (kvp.Value.Func == null)
                        {
                            val = outProp.GetValue(list[i]);
                        }
                        else
                        {
                            val = kvp.Value.Func.Invoke(list[i]);
                        }
                        string cell_value = val == null ? "" : val.ToString();
                        if (kvp.Value.IsImage)
                        {
                            AddCellPicture(sheet, wb, cell_value, cell_Conent.Address.Row, cell_Conent.Address.Row, cell_Conent.Address.Column, cell_Conent.Address.Column);
                        }
                        else
                        {
                            cell_Conent.SetCellValue(cell_value);
                        }

                        jj++;
                    }

                }
            }
            MemoryStream stream = new MemoryStream();
            wb.Write(stream);
            return stream.ToArray();
        }


        /// <summary>
        /// 将Excel导入到List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fs">Stream 文件流</param>
        /// <param name="list">转换的Dictionary
        /// <returns></returns>
        public List<T> ExcelToList<T>(Stream fs, Dictionary<string, ParamImportToList> list) where T : class, new()
        {
            List<T> ts = new List<T>();
            NPOI.SS.UserModel.IWorkbook workbook = null;
            NPOI.SS.UserModel.ISheet sheet = null;
            List<string> listName = new List<string>();
            try
            {
                // 获得此模型的公共属性
                var propertys = typeof(T).GetProperties();
                Dictionary<string, PropertyInfo> dictProps = new Dictionary<string, PropertyInfo>();
                foreach (PropertyInfo property in propertys)
                {
                    dictProps.Add(property.Name, property);
                }
                workbook = new HSSFWorkbook(fs);

                if (workbook != null)
                {
                    sheet = workbook.GetSheetAt(0);//读取第一个sheet，当然也可以循环读取每个sheet

                    if (sheet != null)
                    {
                        int rowCount = sheet.LastRowNum;//总行数
                        if (rowCount > 0)
                        {
                            NPOI.SS.UserModel.IRow firstRow = sheet.GetRow(0);//第一行
                            int cellCount = firstRow.LastCellNum;//列数
                            //循环列数
                            for (int i = 0; i < cellCount; i++)
                            {
                                //添加到listname
                                listName.Add(firstRow.GetCell(i).StringCellValue);
                            }

                            for (int i = 1; i <= rowCount; i++)
                            {
                                T t = new T();
                                NPOI.SS.UserModel.IRow currRow = sheet.GetRow(i);//第i行

                                for (int k = 0; k < cellCount; k++)
                                {   //取值
                                    string value = null;
                                    if (currRow.GetCell(k) != null)
                                    {
                                        firstRow.GetCell(0).SetCellType(CellType.String);
                                        currRow.GetCell(k).SetCellType(CellType.String);
                                        value = currRow.GetCell(k).StringCellValue;
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                    //获取第k列表头名
                                    var Name = listName[k];
                                    ParamImportToList outval;
                                    if (list.TryGetValue(Name, out outval))
                                    {
                                        PropertyInfo outprop;
                                        if (dictProps.TryGetValue(outval.Field, out outprop))
                                        {
                                            outprop.SetValue(t, outval.Func.Invoke(value), null);
                                        }
                                    }

                                }
                                //对象添加到泛型集合中
                                ts.Add(t);
                            }
                        }
                    }
                }
                return ts;
            }
            catch (Exception ex)
            {
                if (fs != null)
                {
                    fs.Close();
                }
                return null;
            }
        }


        private async void AddCellPicture(NPOI.SS.UserModel.ISheet sheet, HSSFWorkbook workbook, string fileUrl, int firstRow, int firstCol, int lastRow, int lastCol)
        {
            try
            {
                //校验地址
                if (fileUrl == null || fileUrl == "" || fileUrl.IndexOf("/") == -1 || fileUrl.IndexOf(".") == -1)
                {
                    return;
                }

                string[] arr = fileUrl.Split(new Char[] { '/' });
                string fileName = arr[arr.Length - 1];
                string fullPath = null;
                GeneralOption go = _provider.GetService<IOptions<GeneralOption>>().Value;
                string tmpPath = fileUrl;
                if (!string.IsNullOrEmpty(go.url))
                {
                    int startIdx = go.url.IndexOf("://");
                    if (startIdx != -1)
                    {
                        string tmpuurl = go.url.Substring(startIdx);
                        if (tmpPath.StartsWith("https"))
                        {
                            tmpPath = tmpPath.Substring(5).Replace(tmpuurl, string.Empty);
                        }
                        else
                        {
                            tmpPath = tmpPath.Substring(4).Replace(tmpuurl, string.Empty);
                        }
                    }
                }
                if (tmpPath.StartsWith("/"))
                {
                    IWebHostEnvironment env = _provider.GetService<IWebHostEnvironment>();
                    fullPath = TAUtility.RelativeToAbsolutePath(env.WebRootPath, tmpPath);
                }

                byte[] bytes = null;
                if (fullPath != null && !File.Exists(fullPath))
                {
                    //本地没有，下载文件保存到本地
                    var fileHelper = _provider.GetService<FileHelper>();
                    bytes = await fileHelper.DownFile(fileUrl);
                }
                else
                {
                    //从本地取
                    bytes = System.IO.File.ReadAllBytes(fullPath);
                }

                var drawing = sheet.CreateDrawingPatriarch();
                int pictureIdx = -1;
                if (fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                {
                    pictureIdx = workbook.AddPicture(bytes, NPOI.SS.UserModel.PictureType.PNG);
                }
                else
                {
                    pictureIdx = workbook.AddPicture(bytes, NPOI.SS.UserModel.PictureType.JPEG);
                }
                HSSFClientAnchor anchor = new HSSFClientAnchor(0, 0, 0, 0, firstCol, firstRow, lastCol, lastRow);
                drawing.CreatePicture(anchor, pictureIdx);
                return;
            }
            catch (Exception e) { }
        }

    }

    public class ParamRenderToExcel<T>
    {
        /// <summary>
        /// 中文名
        /// </summary>
        public string Name { get; set; }
        public Func<T, object> Func { get; set; }
        public bool IsImage { get; set; }
        public ParamRenderToExcel(string name, Func<T, object> func, bool isImage = false)
        {
            Name = name;
            Func = func;
            IsImage = isImage;
        }
        public ParamRenderToExcel(string name, bool isImage = false)
        {
            Name = name;
            Func = null;
            IsImage = isImage;
        }
    }
    public class ParamImportToList
    {
        public string Field { get; set; }
        public Func<string, object> Func { get; set; }
        public ParamImportToList(string field, Func<string, object> func)
        {
            Field = field;
            Func = func;
        }
        public ParamImportToList(string field)
        {
            Field = field;
            Func = x => x;
        }
    }
}

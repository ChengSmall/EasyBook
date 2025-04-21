using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Cheng.Json;
using Cheng.Algorithm.Collections;
using System.Linq;

namespace Cheng.EasyBooks
{

    /// <summary>
    /// easybook书籍标头
    /// </summary>
    public struct EasyBookHeader
    {

        #region 结构

        /// <summary>
        /// easybook书记表头的cover内容
        /// </summary>
        public struct Cover
        {
            public Cover(float width, float height)
            {
                this.width = width;
                this.height = height;
            }

            /// <summary>
            /// 长度比
            /// </summary>
            public float width;

            /// <summary>
            /// 宽度比
            /// </summary>
            public float height;

        }

        #endregion

        #region 参数

        /// <summary>
        /// 书名
        /// </summary>
        public string name;

        /// <summary>
        /// 作者，null表示没有
        /// </summary>
        public string author;

        /// <summary>
        /// 版权信息，null表示没有
        /// </summary>
        public string publisher;

        /// <summary>
        /// 书籍标签，null表示没有
        /// </summary>
        public string[] tags;

        /// <summary>
        /// 封面图路径，null表示没有封面
        /// </summary>
        public string coverImage;

        /// <summary>
        /// 简介，null表示没有
        /// </summary>
        public string synopsis;

        /// <summary>
        /// 自定义书籍标签，null表示没有标签
        /// </summary>
        public KeyValuePair<string, string>[] customs;

        /// <summary>
        /// 书籍封面的长宽比，没有则为null
        /// </summary>
        public Cover? cover;

        #endregion

        #region json

        /// <summary>
        /// 将json对象数据转化为easybook书籍头
        /// </summary>
        /// <param name="json">json数据</param>
        /// <returns>转化后的数据</returns>
        /// <exception cref="NotImplementedException">无法转化</exception>
        public static EasyBookHeader JsonToEasyBookHeader(JsonVariable json)
        {

            try
            {
                StringBuilder sBuf = null;
                EasyBookHeader header = default;

                var headRoot = json.JsonObject;

                 header.name = headRoot["name"].String;
                if (header.name is null) throw new NotImplementedException(Cheng.Properties.Resources.Exception_JsonFormatError);

                if(headRoot.TryGetValue("author", out json))
                {
                    if(json.DataType == JsonType.String)
                    {
                        if(json.String.Length != 0) header.author = json.String;
                    }
                }

                if (headRoot.TryGetValue("publisher", out json))
                {
                    if (json.DataType == JsonType.String)
                    {
                        if (json.String.Length != 0) header.publisher = json.String;
                    }
                }

                if (headRoot.TryGetValue("tags", out json))
                {
                    if (json.DataType == JsonType.List)
                    {
                        var tagsObj = json.Array;
                        if (tagsObj.Count != 0)
                        {
                            header.tags = 
                                tagsObj.ToOtherItemsByCondition<JsonVariable, string>(JsonToString).ToArray();

                        }
                    }

                }

                if (headRoot.TryGetValue("synopsis", out json))
                {
                    if(json.DataType == JsonType.List)
                    {
                        var sysArr = json.Array;
                        if(sBuf is null) sBuf = new StringBuilder(sysArr.Count * 5);
                        else sBuf.Clear();
                        for (int i = 0; i < sysArr.Count; i++)
                        {
                            if(i + 1 == sysArr.Count)
                            {
                                sBuf.Append(sysArr[i].String);
                            }
                            else
                            {
                                sBuf.AppendLine(sysArr[i].String);
                            }

                        }
                        //foreach (var lineSys in sysArr)
                        //{
                        //    sBuf.AppendLine(lineSys.String);
                        //}
                        header.synopsis = sBuf.ToString();
                    }
                    else if(json.DataType == JsonType.String)
                    {
                        //字符串类型
                        header.synopsis = json.String;
                    }
                }

                if (headRoot.TryGetValue("custom", out json))
                {

                    if(json.DataType == JsonType.Dictionary)
                    {
                        var customObj = json.JsonObject;
                        if(customObj.Count != 0)
                        {
                            //有自定义标签
                            var cstags = new Dictionary<string, string>(customObj.Count);

                            foreach (var customTag in customObj)
                            {
                                cstags.Add(customTag.Key, customTag.Value.String);
                            }

                            header.customs = cstags.ToArray();
                        }

                    }

                }

                if (headRoot.TryGetValue("cover", out json))
                {
                    if (json.DataType == JsonType.Dictionary)
                    {

                        var coverObj = json.JsonObject;

                        Cover cover = default;
                        json = coverObj["width"];
                        if(json.DataType == JsonType.Integer)
                        {
                            cover.width = json.Integer;
                        }
                        else
                        {
                            cover.width = (float)json.RealNum;
                        }

                        json = coverObj["height"];
                        if (json.DataType == JsonType.Integer)
                        {
                            cover.height = json.Integer;
                        }
                        else
                        {
                            cover.height = (float)json.RealNum;
                        }
                    }
                }

                return header;

            }
            catch(NotImplementedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }


            
        }

        /// <summary>
        /// 将json数据转化为字符串的方法
        /// </summary>
        /// <param name="json">json数据</param>
        /// <param name="reValue">转化后的字符串</param>
        /// <returns>如果是字符串类型json则成功转化并返回true</returns>
        public static bool JsonToString(JsonVariable json, out string reValue)
        {
            if ((json?.DataType).GetValueOrDefault(JsonType.Null) == JsonType.String)
            {
                reValue = json.String;
                return true;
            }
            reValue = null;
            return false;
        }

        #endregion

        #region 派生

        public override string ToString()
        {
            return name;
        }

        #endregion

    }

}

using Cheng.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cheng.EasyBooks
{

    /// <summary>
    /// 分卷类型
    /// </summary>
    public enum VolumeType : byte
    {
        /// <summary>
        /// 没有任何子分卷和章节内容的空分卷
        /// </summary>
        Empty = 0,

        /// <summary>
        /// 章节分卷
        /// </summary>
        /// <remarks>分卷内包含若干章节</remarks>
        Chapters = 1,

        /// <summary>
        /// 节点分卷
        /// </summary>
        /// <remarks>包含若干子分卷的分卷节点</remarks>
        Node = 2
    }

    /// <summary>
    /// 一个分卷数据
    /// </summary>
    public struct Volume
    {

        #region 参数

        /// <summary>
        /// 分卷名
        /// </summary>
        public string name;

        /// <summary>
        /// 分卷引用的文件夹
        /// </summary>
        public string index;

        /// <summary>
        /// 分卷简介，null表示不存在
        /// </summary>
        public string synopsis;

        /// <summary>
        /// 分卷的自定义标签，null表示不存在
        /// </summary>
        public KeyValuePair<string, string>[] customItems;

        /// <summary>
        /// 当前分卷章节，仅在<see cref="volumeType"/>是<see cref="VolumeType.Chapters"/>时有效，null表示不存在
        /// </summary>
        /// <remarks>从起始索引到最后，每一个元素所指向的位置代表一个章节</remarks>
        public List<ChapterIndex> chapters;

        /// <summary>
        /// 分卷类型
        /// </summary>
        public VolumeType volumeType;

        #endregion

        #region 

        /// <summary>
        /// 将volumes下的一个元素初始化为分卷数据
        /// </summary>
        /// <param name="jsonVolumeItem">volumes数据中的一项</param>
        /// <param name="stringBuilder">暂时存储字符串的缓冲区</param>
        /// <returns>一个分卷数据索引</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="NotImplementedException">参数不是easybook规定的格式</exception>
        public static Volume JsonToVolume(JsonVariable jsonVolumeItem, StringBuilder stringBuilder)
        {
            if (jsonVolumeItem is null) throw new ArgumentNullException();
            if (stringBuilder is null) stringBuilder = new StringBuilder();
            try
            {
                Volume vol = default;

                var baseObj = jsonVolumeItem.JsonObject;
                vol.index = baseObj["index"].String;
                vol.name = baseObj["name"].String;
                JsonVariable json;

                if (baseObj.TryGetValue("synopsis", out json))
                {
                    if(json.DataType == JsonType.String)
                    {
                        if (json.String.Length != 0)
                        {
                            vol.synopsis = json.String;
                        }
                    }
                    else if (json.DataType == JsonType.List)
                    {

                        var jsonLines = json.Array;
                        if (jsonLines.Count != 0)
                        {
                            stringBuilder.Clear();
                            int count = jsonLines.Count;
                            for (int i = 0; i < count; i++)
                            {
                                if(i + 1 == count)
                                {
                                    stringBuilder.Append(jsonLines[i].String);
                                }
                                else
                                {
                                    stringBuilder.AppendLine(jsonLines[i].String);
                                }
                            }

                            vol.synopsis = stringBuilder.ToString();
                        }

                    }

                }

                if (baseObj.TryGetValue("custom", out json))
                {

                    if (json.DataType == JsonType.Dictionary)
                    {

                        var customObj = json.JsonObject;
                        vol.customItems = new KeyValuePair<string, string>[customObj.Count];
                        int i = 0;
                        foreach (var item in customObj)
                        {
                            vol.customItems[i] = new KeyValuePair<string, string>(item.Key, item.Value.String);
                            i++;
                        }
                    }

                }

                return vol;
            }
            catch (NotImplementedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }
        }

        /// <summary>
        /// 将一个Volume.json文件的json数据转化为分卷集合
        /// </summary>
        /// <param name="jsonVolumes">一个Volume.json文件的json数据</param>
        /// <param name="stringBuilder">字符缓冲区</param>
        /// <param name="volCount">提前获取分卷数量</param>
        /// <returns>可迭代出分卷数据的枚举器，每次推进都将返回一个分卷数据，如果返回null表示此次的数据有错误；返回的分卷数据不包括章节数据</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="NotImplementedException">解析数据错误</exception>
        public static IEnumerable<Volume?> JsonToVolumes(JsonVariable jsonVolumes, StringBuilder stringBuilder, out int volCount)
        {
            if (jsonVolumes is null) throw new ArgumentNullException();

            if(jsonVolumes.DataType == JsonType.Dictionary)
            {
                var jobj = jsonVolumes.JsonObject;
                if (jobj.TryGetValue("volumes", out var JsonArr))
                {
                    if(JsonArr.DataType == JsonType.List)
                    {
                        volCount = JsonArr.Array.Count;
                        return f_jsonToVolumes(JsonArr.Array, stringBuilder ?? new StringBuilder());
                    }
                }
            }

            throw new NotImplementedException(Cheng.Properties.Resources.Exception_JsonFormatError);
        }

        internal static IEnumerable<Volume?> f_jsonToVolumes(JsonList jsonVolumes, StringBuilder stringBuilder)
        {

            int length = jsonVolumes.Count;
            Volume vols = default;

            for (int i = 0; i < length; i++)
            {
                var jobj = jsonVolumes[i];
                bool notError = true;
                
                try
                {
                    vols = JsonToVolume(jobj, stringBuilder);
                }
                catch (Exception)
                {
                    notError = false;
                }

                if (notError) yield return vols;
                else yield return null;
            }


        }

        /// <summary>
        /// 分卷名
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return name;
        }

        #endregion

    }


}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace KaSon.FrameWork.Common.Utilities
{
    public static class AreaHelper
    {
        private static IList<Province> _provinceList = null;
        public static IList<Province> GetAllProvinces()
        {
            if (_provinceList == null || _provinceList.Count == 0)
            {
                var configFileName = @"Settings\common_area_provinces.config.xml";
                if (!File.Exists(configFileName))
                {
                    throw new FileNotFoundException("省份配置文件不存在 - " + configFileName);
                }
                using (var stream = File.OpenRead(configFileName))
                {
                    _provinceList = GetAllProvinces(stream);
                }
            }
            return _provinceList;
        }
        public static Province GetProvinceByName(string name)
        {
            if (name.EndsWith("省"))
            {
                name = name.TrimEnd('省');
            }
            var list = GetAllProvinces();
            foreach (var item in list)
            {
                if (item.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return item;
                }
            }
            return null;
        }
        private static IList<Province> GetAllProvinces(Stream xmlStream)
        {
            var list = new List<Province>();
            var xml = new XmlDocument();
            xml.Load(xmlStream);
            var nodeList = xml.SelectNodes("provinces/province");
            foreach (XmlElement item in nodeList)
            {
                var province = new Province();
                var e_id = item.SelectSingleNode("id");
                var e_name = item.SelectSingleNode("name");
                province.Id = e_id.InnerText;
                province.Name = e_name.InnerText;

                list.Add(province);
            }
            return list;
        }

        private static Dictionary<string, IList<City>> _cityDict = new Dictionary<string, IList<City>>();
        public static IList<City> GetCitiesByProvince(string provinceId)
        {
            if (!_cityDict.ContainsKey(provinceId))
            {
                var configFileName = @"Settings\common_area_cities.config.xml";
                if (!File.Exists(configFileName))
                {
                    throw new FileNotFoundException("城市配置文件不存在 - " + configFileName);
                }
                using (var stream = File.OpenRead(configFileName))
                {
                    var cityList = GetCitiesByProvince(stream, provinceId);
                    if (cityList != null && cityList.Count > 0)
                    {
                        _cityDict.Add(provinceId, cityList);
                    }
                }
            }
            return _cityDict[provinceId];
        }
        private static IList<City> GetCitiesByProvince(Stream xmlStream, string provinceId)
        {
            var list = new List<City>();
            var xml = new XmlDocument();
            xml.Load(xmlStream);
            var nodeList = xml.SelectNodes("cities/city");
            foreach (XmlElement item in nodeList)
            {
                var e_province_id = item.SelectSingleNode("province_id");
                var province = e_province_id.InnerText;
                if (!province.Equals(provinceId))
                {
                    continue;
                }
                var e_id = item.SelectSingleNode("id");
                var e_name = item.SelectSingleNode("name");
                var e_code = item.SelectSingleNode("code");

                var city = new City();
                city.Id = e_id.InnerText;
                city.Name = e_name.InnerText;
                city.ProvinceId = province;
                city.Code = e_code.InnerText;

                list.Add(city);
            }
            return list;
        }
    }
    public class Province
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
    public class City
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string ProvinceId { get; set; }
    }
}

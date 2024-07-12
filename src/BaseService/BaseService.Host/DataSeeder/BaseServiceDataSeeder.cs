using BaseService.BaseData;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;

namespace BaseService.DataSeeder
{
    public class BaseServiceDataSeeder : IDataSeedContributor, ITransientDependency
    {
        private readonly IRepository<DataDictionary, Guid> _dataDicRepository;
        private readonly IRepository<DataDictionaryDetail, Guid> _dataDicDetailRepository;
        private readonly IGuidGenerator _guidGenerator;

        public BaseServiceDataSeeder(
            IRepository<DataDictionary, Guid> dataDicRepository,
            IRepository<DataDictionaryDetail, Guid> dataDicDetailRepository,
            IGuidGenerator guidGenerator
            )
        {
            _dataDicRepository = dataDicRepository;
            _dataDicDetailRepository = dataDicDetailRepository;
            _guidGenerator = guidGenerator;
        }

        public virtual async Task SeedAsync(DataSeedContext context)
        {
            #region 数据库写入创建默认数据

            //await CreateDataDictionaries();

            #endregion
        }

        // 创建数据字典
        private async Task CreateDataDictionaries()
        {
            // 字典数据
            var dictionaryMap = new Dictionary<string, string>
            {
                { "accessProtocol", "接入协议" },
                { "deviceState", "设备状态" },
                { "deviceType", "设备类型" },
              /*  { "topic", "设备主题" },
                { "topicType", "设备主题类型" },*/
                { "unit", "单位" },
                { "ValueType", "数据类型" },
                { "warningLevel", "预警等级" },
                { "warningMethod", "预警方式" }
            };
            // 查询已存在的数据
            var list = await _dataDicRepository.GetListAsync(e => dictionaryMap.Keys.Contains(e.Name));
            if (!list.Any()) return;

            // 过滤掉已存在的字典数据
            var newList = dictionaryMap.Keys.Except(list.Select(e => e.Name)).ToList();
            
            foreach (var item in newList)
            {
                var id = _guidGenerator.Create();
                await _dataDicRepository.InsertAsync(new DataDictionary(id, null, item, dictionaryMap[item], null));
                await CreateDataDictionaryDetails(id, item);
            }
        }

        private async Task CreateDataDictionaryDetails(Guid pid, string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return;
            }
            // 根据关键字判断添加数据
            if ("accessProtocol".Equals(key))
            {
                await _dataDicDetailRepository.InsertAsync(new DataDictionaryDetail(_guidGenerator.Create(), null, pid, "mqtt", "0", 0, null));
            } else if ("deviceState".Equals(key)) {
                await _dataDicDetailRepository.InsertAsync(new DataDictionaryDetail(_guidGenerator.Create(), null, pid, "未知", "0", 0, null));
                await _dataDicDetailRepository.InsertAsync(new DataDictionaryDetail(_guidGenerator.Create(), null, pid, "在线", "1", 1, null));
                await _dataDicDetailRepository.InsertAsync(new DataDictionaryDetail(_guidGenerator.Create(), null, pid, "离线", "2", 2, null));
            } else if ("deviceType".Equals(key)) {
                await _dataDicDetailRepository.InsertAsync(new DataDictionaryDetail(_guidGenerator.Create(), null, pid, "直连设备", "0", 0, null));
                await _dataDicDetailRepository.InsertAsync(new DataDictionaryDetail(_guidGenerator.Create(), null, pid, "网关设备", "1", 1, null));
                await _dataDicDetailRepository.InsertAsync(new DataDictionaryDetail(_guidGenerator.Create(), null, pid, "网关子设备", "2", 2, null));
            }/* else if ("topic".Equals(key)) {
                await _dataDicDetailRepository.InsertAsync(new DataDictionaryDetail(_guidGenerator.Create(), null, pid, "数据上报", "1", 0, null));
                await _dataDicDetailRepository.InsertAsync(new DataDictionaryDetail(_guidGenerator.Create(), null, pid, "命令下发", "0", 1, null));
                await _dataDicDetailRepository.InsertAsync(new DataDictionaryDetail(_guidGenerator.Create(), null, pid, "OTA升级", "0", 2, null));
            } else if ("topicType".Equals(key)) {
                await _dataDicDetailRepository.InsertAsync(new DataDictionaryDetail(_guidGenerator.Create(), null, pid, "发布", "0", 0, null));
                await _dataDicDetailRepository.InsertAsync(new DataDictionaryDetail(_guidGenerator.Create(), null, pid, "订阅", "1", 1, null));
            }*/ else if ("unit".Equals(key)) {
                await _dataDicDetailRepository.InsertAsync(new DataDictionaryDetail(_guidGenerator.Create(), null, pid, "kW", "kW", 0, null));
                await _dataDicDetailRepository.InsertAsync(new DataDictionaryDetail(_guidGenerator.Create(), null, pid, "mg/L", "mg/L", 1, null));
                await _dataDicDetailRepository.InsertAsync(new DataDictionaryDetail(_guidGenerator.Create(), null, pid, "m³", "m³", 2, null));
                await _dataDicDetailRepository.InsertAsync(new DataDictionaryDetail(_guidGenerator.Create(), null, pid, "m³/h", "m³/h", 3, null));
                await _dataDicDetailRepository.InsertAsync(new DataDictionaryDetail(_guidGenerator.Create(), null, pid, "℃", "℃", 4, null));
                await _dataDicDetailRepository.InsertAsync(new DataDictionaryDetail(_guidGenerator.Create(), null, pid, "kW·h", "kW·h", 5, null));
                await _dataDicDetailRepository.InsertAsync(new DataDictionaryDetail(_guidGenerator.Create(), null, pid, "m", "m", 6, null));
            } else if ("ValueType".Equals(key)) {
                await _dataDicDetailRepository.InsertAsync(new DataDictionaryDetail(_guidGenerator.Create(), null, pid, "double", "double", 0, null));
                await _dataDicDetailRepository.InsertAsync(new DataDictionaryDetail(_guidGenerator.Create(), null, pid, "bool", "bool", 1, null));
                await _dataDicDetailRepository.InsertAsync(new DataDictionaryDetail(_guidGenerator.Create(), null, pid, "string", "string", 2, null));
                await _dataDicDetailRepository.InsertAsync(new DataDictionaryDetail(_guidGenerator.Create(), null, pid, "Int", "Int", 3, null));
            } else if ("warningLevel".Equals(key)) {
                await _dataDicDetailRepository.InsertAsync(new DataDictionaryDetail(_guidGenerator.Create(), null, pid, "低等级", "低等级", 0, null));
                await _dataDicDetailRepository.InsertAsync(new DataDictionaryDetail(_guidGenerator.Create(), null, pid, "中等级", "中等级", 1, null));
                await _dataDicDetailRepository.InsertAsync(new DataDictionaryDetail(_guidGenerator.Create(), null, pid, "高等级", "高等级", 2, null));
                await _dataDicDetailRepository.InsertAsync(new DataDictionaryDetail(_guidGenerator.Create(), null, pid, "灾难级", "灾难级", 3, null));
            } else if ("warningMethod".Equals(key)) {
                await _dataDicDetailRepository.InsertAsync(new DataDictionaryDetail(_guidGenerator.Create(), null, pid, "微信公众号", "微信公众号", 0, null));
                await _dataDicDetailRepository.InsertAsync(new DataDictionaryDetail(_guidGenerator.Create(), null, pid, "api推送", "api推送", 1, null));
                await _dataDicDetailRepository.InsertAsync(new DataDictionaryDetail(_guidGenerator.Create(), null, pid, "短信", "短信", 2, null));
                await _dataDicDetailRepository.InsertAsync(new DataDictionaryDetail(_guidGenerator.Create(), null, pid, "邮件", "邮件", 3, null));
            } else {
                Log.Error($"字典明细初始化失败，字典关键字为：{key}");
            }
        }
    }
}

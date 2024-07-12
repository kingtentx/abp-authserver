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
        }
    }
}

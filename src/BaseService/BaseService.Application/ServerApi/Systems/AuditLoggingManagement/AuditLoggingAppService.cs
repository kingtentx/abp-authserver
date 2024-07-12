using BaseService.Permissions;
using BaseService.Systems.AuditLoggingManagement.Dto;
using Cimc.Model.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.AuditLogging;

namespace BaseService.Systems.AuditLoggingManagement
{
    [Area("Base")]
    [Route("api/base/audit-logging")]
    [Authorize(BaseServicePermissions.AuditLogging.Default)]
    public class AuditLoggingAppService : ApplicationService, IAuditLoggingAppService
    {
        private readonly IAuditLogRepository _auditLogRepository;
        public AuditLoggingAppService(
            IAuditLogRepository auditLogRepository)
        {
            _auditLogRepository = auditLogRepository;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ResultDto<Guid>> Get(Guid id)
        {
            var result = new ResultDto<Guid>();

            var auditLog = await _auditLogRepository.GetAsync(id);

            // var dto = ObjectMapper.Map<AuditLog, AuditLogDto>(auditLog);

            result.SetData(auditLog.Id);
            return result;
        }

        [HttpGet]
        [Route("list")]
        public async Task<ResultDto<PagedResultDto<AuditLogDto>>> GetList(GetAuditLogsInput input)
        {
            var result = new ResultDto<PagedResultDto<AuditLogDto>>();

            var count = await _auditLogRepository.GetCountAsync(httpMethod: input.HttpMethod, url: input.Url,
                userName: input.UserName, applicationName: input.ApplicationName, correlationId: input.CorrelationId, maxExecutionDuration: input.MaxExecutionDuration,
                minExecutionDuration: input.MinExecutionDuration, hasException: input.HasException, httpStatusCode: input.HttpStatusCode);
            var list = await _auditLogRepository.GetListAsync(sorting: input.Sorting, maxResultCount: input.MaxResultCount, skipCount: input.SkipCount, httpMethod: input.HttpMethod, url: input.Url,
                userName: input.UserName, applicationName: input.ApplicationName, correlationId: input.CorrelationId, maxExecutionDuration: input.MaxExecutionDuration,
                minExecutionDuration: input.MinExecutionDuration, hasException: input.HasException, httpStatusCode: input.HttpStatusCode);

            var data = new PagedResultDto<AuditLogDto>(
                count,
                ObjectMapper.Map<List<AuditLog>, List<AuditLogDto>>(list)
            );

            result.SetData(data);
            return result;
        }

        [HttpGet]
        [Route("averageExecutionDurationPerDay")]
        public async Task<ResultDto<GetAverageExecutionDurationPerDayOutput>> GetAverageExecutionDurationPerDay(GetAverageExecutionDurationPerDayInput input)
        {
            var result = new ResultDto<GetAverageExecutionDurationPerDayOutput>();

            var query = await _auditLogRepository.GetAverageExecutionDurationPerDayAsync(input.StartDate, input.EndDate);
            var data = new GetAverageExecutionDurationPerDayOutput()
            {
                Data = query
            };

            result.SetData(data);
            return result;
        }
    }
}

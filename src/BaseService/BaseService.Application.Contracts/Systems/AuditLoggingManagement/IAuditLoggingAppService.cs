using BaseService.Systems.AuditLoggingManagement.Dto;
using Cimc.Model.Base;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace BaseService.Systems.AuditLoggingManagement
{
    public interface IAuditLoggingAppService : IApplicationService
    {
        Task<ResultDto<Guid>> Get(Guid id);

        Task<ResultDto<PagedResultDto<AuditLogDto>>> GetList(GetAuditLogsInput input);

        Task<ResultDto<GetAverageExecutionDurationPerDayOutput>> GetAverageExecutionDurationPerDay(GetAverageExecutionDurationPerDayInput input);
    }
}

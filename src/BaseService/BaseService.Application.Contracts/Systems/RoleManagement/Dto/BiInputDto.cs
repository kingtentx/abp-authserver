namespace BaseService.Systems.RoleManagement.Dto
{
    public class BiInputDto
    {
        public string UserId { get; set; }

        public string BaseUrl { get; set; }

        public BiInputDto(string userid, string baseurl)
        {
            UserId = userid;
            BaseUrl = baseurl;
        }
    }
}

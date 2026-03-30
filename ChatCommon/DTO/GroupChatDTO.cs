namespace ChatCommon.DTO
{
    public class GroupChatDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public List<UserDTO> Memebers { get; set; }
    }
}

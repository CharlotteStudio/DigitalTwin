public class UserProfile : ManagerBase<UserProfile>
{
    private string _userName;

    public string GetUserName() => _userName;
    public string SetUserName(string str) => _userName = str;
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}

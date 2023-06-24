public static class MyConstant
{
    public const string MainScene = "MainScene";
    
    public static class SaveKey
    {
        public const string UserName = "UserName";
        public const string Password = "Password";
        public const string FarmlandPosition = "FarmlandPosition";
        public const string DevicePosition = "DevicePosition";
    }
        
    public static class AWSService
    {
        public const string AccessKey = "AKIA2LAI6V2UHDIMTQFJ";
        public const string SecretKey = "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";
        public const string Region = "ap-northeast-1";
        
        public static class LambdaFunction
        {
            public const string CheckUser = "IBSP_CheckUser";
            public const string GetDeviceCurrentValue = "IBSP_GetDeviceCurrentValue";
            public const string GetDeviceValue = "IBSP_GetDeviceValue";
            public const string GetDeviceState = "IBSP_GetDeviceState";
            public const string GetDeviceActive = "IBSP_GetDeviceActive";
            public const string GetDeviceSetting = "IBSP_GetDeviceSetting";
            public const string SetDeviceSetting = "IBSP_SetDeviceSetting";
        }
    }
}

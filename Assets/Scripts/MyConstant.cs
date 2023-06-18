public static class MyConstant
{
    public const string UserName = "testing@gmail.com";
    public const string Password = "123456";
    public const string MainScene = "MainScene";
    
    public static class SaveKey
    {
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
            public const string GetDeviceCurrentValue = "IBSP_GetDeviceCurrentValue";
            public const string GetDeviceValue = "IBSP_GetDeviceValue";
            public const string GetDeviceState = "IBSP_GetDeviceState";
            public const string GetDeviceActive = "IBSP_GetDeviceActive";
            public const string SetDeviceSetting = "IBSP_SetDeviceSetting";
        }
    }
}

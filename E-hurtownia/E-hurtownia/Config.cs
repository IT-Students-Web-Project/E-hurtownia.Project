using E_hurtownia.Utils;

namespace E_hurtownia
{
    public static class Config
    {
        public static void Init()
        {
            ConfigUtil.SetVersion("0.003");
            ConfigUtil.SetConfigIniPath("config.ini");
        }
    }
}

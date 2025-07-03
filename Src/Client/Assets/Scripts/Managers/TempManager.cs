using Models;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Managers
{
    class TempManager : Singleton<TempManager>
    {
        public void Init()
        {

        }

        public void UpdateTempInfo(NTempInfo info)
        {
            User.Instance.TempInfo = info;
            this.ShowTempUI(info != null);
        }

        public void ShowTempUI(bool show)
        {
            if (UIMain.Instance != null)
            {
                UIMain.Instance.ShowTeamUI(show);
            }
        }
    }
}

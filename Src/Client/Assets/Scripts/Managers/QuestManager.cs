using Assets.Scripts.Models;
using Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Managers
{
    public class QuestManager : Singleton<QuestManager>
    {

        public Dictionary<int, Quest> allQuests = new Dictionary<int, Quest>();
    }
}

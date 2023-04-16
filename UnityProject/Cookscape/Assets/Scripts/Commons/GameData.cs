
using System.Collections.Generic;
using UnityEngine;
using static UnityProject.Cookscape.AvatarData;
using static UnityProject.Cookscape.ChallengeData;

namespace UnityProject.Cookscape
{
    #region RECEIVE_FORM
    public class ItemForm
    {
        public List<ItemData> body;
    }

    public class ObjectForm
    {
        public List<ObjectData> body;
    }

    public class AvatarForm
    {
        public List<AvatarData> body;
    }

    public class ChallengeForm
    {
        public List<ChallengeData> body;
    }

    public class RewardForm
    {
        public List<RewardData> body;
    }

    public class UsageAvatarForm
    {
        public List<UsageAvatarData> body;
    }

    public class VersionForm
    {
        public float body;
    }

    public class UserIDForm
    {
        public long body; 
    }

    public class UserForm
    {
        public User body;
    }

    public class DataForm
    {
        public UserData body;
    }

    #endregion

    #region SEND_FORM

    public class LoginForm
    {
        public string loginId { get; set; }
        public string password { get; set; }

        public LoginForm(string loginId, string password)
        {
            this.loginId = loginId;
            this.password = password;
        }
    }

    public class GameResultForm
    {
        public long avatarId { get; set;}
        public bool winFlag { get; set; }
        public int exp { get; set; }
        public int rankPoint { get; set; }
        public int level { get; set; }
        public int money { get; set; }
        public int saveCount { get; set; }
        public int catchCount { get; set; }
        public int catchedCount { get; set; }
        public int valveOpenCount { get; set; }
        public int valveCloseCount { get; set; }
        public int potDestroyCount { get; set; }
        public float maxNotUseStaminaTime { get; set; }
        public float maxNotMoveTime { get; set; }
        public int maxCatchCount { get; set; }
        public int hitedCount { get; set; }
        public int maxNotCatchCount { get; set; }

        public GameResultForm(long avatarId, bool winFlag, int exp, int rankPoint, int level, int money, int saveCount,
            int catchCount, int catchedCount, int valveOpenCount, int valveCloseCount, int potDestroyCount, float maxNotUseStaminaTime,
        float maxNotMoveTime, int maxCatchCount, int hitedCount, int maxNotCatchCount)
        {
            this.avatarId = avatarId;
            this.winFlag = winFlag;
            this.exp = exp;
            this.rankPoint = rankPoint;
            this.level = level;
            this.money = money;
            this.saveCount = saveCount;
            this.catchCount = catchCount;
            this.catchedCount = catchedCount;
            this.valveOpenCount = valveOpenCount;
            this.valveCloseCount = valveCloseCount;
            this.potDestroyCount = potDestroyCount;
            this.maxNotUseStaminaTime = maxNotUseStaminaTime;
            this.maxNotMoveTime = maxNotMoveTime;
            this.maxCatchCount = maxCatchCount;
            this.hitedCount = hitedCount;
            this.maxNotCatchCount = maxNotCatchCount;
        }
    }

    public class UserUpdateForm
    {
        public string avatarName { get; set; }
        public string title { get; set; }
        public string hat { get; set; }

        public UserUpdateForm(string avatarName, string title, string hat)
        {
            this.avatarName = avatarName;
            this.title = title;
            this.hat = hat;
        }
    }

    #endregion

    #region INFORMATION
    public class VersionData
    {
        public float version { get; set; }

        public override string ToString()
        {
            return "Version"
                + "[ version : " + version
                + " ]";
        }

        public void Print()
        {
            Debug.Log(this.ToString());
        }
    }

    public class ItemData
    {
        public enum ITEM {
            나무젓가락,
            키친타올,
        }

        public long itemId { get; set; }
        public string name { get; set; }
        public string desc { get; set; }
        public int useCount { get; set; }
        public float weight { get; set; }

        public override string ToString()
        {
            return "Item"
                + "[ itemId:" + itemId
                + ", name:" + name
                + ", desc:" + desc
                + ", useCount:" + useCount
                + ", weight:" + weight
                + " ]";
        }

        public void Print()
        {
            Debug.Log(this.ToString());
        }

    }

    public class ObjectData
    {
        public enum OBJECT {
            냄비,
            밸브,
        }

        public long objectId { get; set; }
        public string name { get; set; }
        public string desc { get; set; }
        public float gauge { get; set; }
        public float chargingSpeed { get; set; }
        public float dechargingSpeed { get; set; }
        public string interactableType { get; set; } // CHEF/RUNNER/ALL
        public float chargingSpeedCoFactor { get; set; }
        public float dechargingSpeedCoFactor { get; set; }

        public override string ToString()
        {
            return "MapObject"
                + "[ objectId:" + objectId
                + ", name:" + name
                + ", desc:" + desc
                + ", gauge:" + gauge
                + ", chargingSpeed:" + chargingSpeed
                + ", dedechargingSpeedsc:" + dechargingSpeed
                + ", interactableType:" + interactableType
                + ", chargingSpeedCoFactor:" + chargingSpeedCoFactor
                + ", dechargingSpeedCoFactor:" + dechargingSpeedCoFactor
                + " ]";
        }

        public void Print()
        {
            Debug.Log(this.ToString());
        }

    }

    public class ChallengeData
    {
        public enum CHALLENGE
        {
            청결이최우선,
            찾았다,
            야호,
            느긋하게걷기,
            나는전설이다,
            대체왜이래요,
            나어디있게,
            세상끝으로,
            다음엔친구랑같이오거라,
            THECOOKSLAYER
        }

        public long challengeId { get; set; }
        public string name { get; set; }
        public string desc { get; set; }
        public bool singleAchievementFlag { get; set; }
        public string firstLevel { get; set; }
        public bool firstAchievementFlag { get; set; }
        public string secondLevel { get; set; }
        public bool secondAchievementFlag { get; set; }
        public string thirdLevel { get; set; }
        public bool thirdAchievementFlag { get; set; }
        public string keyValue { get; set; }

        public override string ToString()
        {
            return "Challenge"
                + "[ challengeId:" + challengeId
                + ", name:" + name
                + ", desc:" + desc
                + ", singleAchievementFlag:" + singleAchievementFlag
                + ", firstLevel:" + firstLevel
                + ", firstAchievementFlag:" + firstAchievementFlag
                + ", secondLevel:" + secondLevel
                + ", secondAchievementFlag:" + secondAchievementFlag
                + ", thirdLevel:" + thirdLevel
                + ", thirdAchievementFlag:" + thirdAchievementFlag
                + ", keyValue:" + keyValue
                + " ]";
        }

        public void Print()
        {
            Debug.Log(this.ToString());
        }
    }

    public class RewardData
    {
        public enum REWARD
        {
            아이깨끗해,
            연쇄세척마,
            등산마니아,
            양반,
            대감,
            전설적인,
            반죽,
            빨래,
            중급닌자,
            상급닌자,
            호카게,
            미개척탐험가,
            주방장,
            자비로운주방장,
            COOKING,
            THECOOKSLAYER,
            보글보글모자,
            물음표의모자,
            GOD,
            혹,
            요리사모자,
            왕관
        }

        public enum TYPE
        {
            TITLE,
            HAT
        }

        public enum GRADE
        {
            NORMAL,
            RARE,
            UNIQUE
        }

        public long rewardId { get; set; }
        public string name { get; set; }
        public string desc { get; set; }
        public string type { get; set; }
        public string grade { get; set; }
        public string keyValue { get; set; }

        public override string ToString()
        {
            return "Reward"
                + "[ userRewardId:" + rewardId
                + ", name:" + name
                + ", desc:" + desc
                + ", type:" + type
                + ", keyValue:" + keyValue
                + " ]";
        }

        public void Print()
        {
            Debug.Log(this.ToString());
        }
    }

    public class AvatarData
    {
        public enum AVATAR
        {
            요리사,
            고기,
            피망,
            토마토,
            콜라
        }

        public long avatarId { get; set; }
        public string name { get; set; }
        public string desc { get; set; }
        public float movementSpeed { get; set; }
        public float rotationSpeed { get; set; }
        public float jumpForce { get; set; }
        public float stamina { get; set; }
        public float staminaDecreasingFactor { get; set; }
        public float staminaIncreasingFactor { get; set; }
        public float interactionMinDist { get; set; }
        public float interactionReadyTime { get; set; }
        public float footprintSpace { get; set; }
        public float speedCoFactor { get; set; }
        public float jumpForceCoFactor { get; set; }

        public override string ToString()
        {
            return "Avatar"
                + "[ avatarId:" + avatarId
                + ", name:" + name
                + ", desc:" + desc
                + ", movementSpeed:" + movementSpeed
                + ", rotationSpeed:" + rotationSpeed
                + ", jumpForce:" + jumpForce
                + ", stamina:" + stamina
                + ", staminaDecreasingFactor:" + staminaDecreasingFactor
                + ", staminaIncreasingFactor:" + staminaIncreasingFactor
                + ", interactionMinDist:" + interactionMinDist
                + ", interactionReadyTime:" + interactionReadyTime
                + ", footprintSpace:" + footprintSpace
                + ", speedCoFactor:" + speedCoFactor
                + ", footprintSpace:" + jumpForceCoFactor
                + " ]";
        }

        public void Print()
        {
            Debug.Log(this.ToString());
        }
    }
    #endregion

    #region DATA
    public class User
    {
        public long userId { get; set; }
        public string nickname { get; set; }
        public string avatarName { get; set; }
        public string title { get; set; }
        public string hat { get; set; }

        public void UpdateUser(UserUpdateForm userUpdateForm)
        {
            avatarName = userUpdateForm.avatarName;
            title = userUpdateForm.title;
            hat = userUpdateForm.hat;
        }

        public void SetUser(User user)
        {
            avatarName = user.avatarName;
            title = user.title;
            hat = user.hat;
        }

        public override string ToString()
        {
            return "User"
                + "[ userId:" + userId
                + ", nickname:" + nickname
                + ", avatarName:" + avatarName
                + ", title:" + title
                + ", hat:" + hat
                + " ]";
        }

        public void Print()
        {
            Debug.Log(this.ToString());
        }
    }

    public class UserData
    {
        public long dataId { get; set; }
        public int exp { get; set; }
        public int rankPoint { get; set; }
        public int level { get; set; }
        public int money { get; set; }
        public int winCount { get; set; }
        public int loseCount { get; set; }
        public int saveCount { get; set; }
        public int catchCount { get; set; }
        public int catchedCount { get; set; }
        public int valveOpenCount { get; set; }
        public int valveCloseCount { get; set; }
        public int potDestroyCount { get; set; }
        public float maxNotUseStaminaTime { get; set; }
        public float maxNotMoveTime { get; set; }
        public int maxCatchCount { get; set; }
        public int hitedCount { get; set; }
        public int maxNotCatchCount { get; set; }

        public void SetUserData(UserData userData)
        {
            exp = userData.exp;
            rankPoint = userData.rankPoint;
            level = userData.level;
            money = userData.money;
            winCount = userData.winCount;
            loseCount = userData.loseCount;
            saveCount = userData.saveCount;
            catchCount = userData.catchCount;
            catchedCount = userData.catchedCount;
            valveOpenCount = userData.valveOpenCount;
            valveCloseCount = userData.valveCloseCount;
            potDestroyCount = userData.potDestroyCount;
            maxNotUseStaminaTime = userData.maxNotUseStaminaTime;
            maxNotMoveTime = userData.maxNotMoveTime;
            maxCatchCount = userData.maxCatchCount;
            hitedCount = userData.hitedCount;
            maxNotCatchCount = userData.maxNotCatchCount;
        }

        public void AddGameResultData(GameResultForm gameResultForm)
        {
            exp = exp + gameResultForm.exp;
            rankPoint = rankPoint + gameResultForm.rankPoint;
            level = gameResultForm.level;
            money = money + gameResultForm.money;
            winCount = gameResultForm.winFlag == true ? winCount + 1 : winCount;
            loseCount = gameResultForm.winFlag == true ? loseCount : loseCount + 1;
            saveCount = saveCount + gameResultForm.saveCount;
            catchCount = catchCount + gameResultForm.catchCount;
            catchedCount = catchedCount + gameResultForm.catchedCount;
            valveOpenCount = valveOpenCount + gameResultForm.valveOpenCount;
            valveCloseCount = valveCloseCount + gameResultForm.valveCloseCount;
            potDestroyCount = potDestroyCount + gameResultForm.potDestroyCount;
            maxNotUseStaminaTime = Mathf.Max( gameResultForm.maxNotUseStaminaTime, maxNotUseStaminaTime);
            maxNotMoveTime = Mathf.Max( gameResultForm.maxNotMoveTime, maxNotMoveTime);
            maxCatchCount = Mathf.Max(gameResultForm.maxCatchCount, maxCatchCount);
            hitedCount += gameResultForm.hitedCount;
            maxNotCatchCount = Mathf.Max(gameResultForm.maxNotCatchCount, maxNotCatchCount);
        }

        public override string ToString()
        {
            return "UserData"
                + "[ dataId:" + dataId
                + ", exp:" + exp
                + ", rankPoint:" + rankPoint
                + ", level:" + level
                + ", money:" + money
                + ", winCount:" + winCount
                + ", loseCount:" + loseCount
                + ", saveCount:" + saveCount
                + ", catchCount:" + catchCount
                + ", catchedCount:" + catchedCount
                + ", valveOpenCount:" + valveOpenCount
                + ", valveCloseCount:" + valveCloseCount
                + ", potDestroyCount:" + potDestroyCount
                + " ]";
        }

        public void Print()
        {
            Debug.Log(this.ToString());
        }
    }

    public class UsageAvatarData
    {
        public string name { get; set; }
        public string desc { get; set; }
        public int useCount { get; set; }

        public void SetUsageAvatarData(UsageAvatarData usageAvatarData)
        {
            useCount = usageAvatarData.useCount;
        }

        public void AddUsage()
        {
            useCount = useCount + 1;
        }

        public override string ToString()
        {
            return "UsageAvatar"
                + "[ name:" + name
                + ", desc:" + desc
                + ", useCount:" + useCount
                + " ]";
        }

        public void Print()
        {
            Debug.Log(this.ToString());
        }
    }

   
    #endregion
   
}



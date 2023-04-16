
namespace UnityProject.Cookscape
{
    //���ڿ� �Է� Ʋ�� ���� �����ϱ� �����س���
    public class GameConstants
    {
        // Http communication url
        public const string API_URL = "https://j8b109.p.ssafy.io/api";
        // public const string API_URL = "https://j8b109.p.ssafy.io/test"; // TEST URL

        //Input.GetAxis() ��
        public const string axisNameVertical = "Vertical";
        public const string axisNameHorizontal = "Horizontal";
        public const string mouseAxisNameVertical = "Mouse Y";
        public const string mouseAxisNameHorizontal = "Mouse X";

        public const string buttonNameJump = "Jump";

        //Animator Check ��
        //�յ� Ű�Է°� ����
        public const string playerVerticalVelocity = "VerticalVelocity";
        //�¿� Ű�Է°� ����
        public const string playerHorizonalVelocity = "HorizonalVelocity";
        //����Trigger
        public const string playerJumpNow = "JumpNow";
        //������
        public const string playerJumpTrigger = "DoJump";
        //������
        public const string playerOnGround = "IsGround";
    }
}

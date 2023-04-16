import { useRef, useState } from "react";
import { axiostInstance } from "../utils/getAxios";
import { motion } from "framer-motion";

import "../css/SignupComponent.css";

import Carousel from "./Carousel";

const className_inputContainer = "inputContainer";

const SignupComponent = ({ togglePage }) => {
  const [userid, setUserid] = useState("");
  const [password, setPassword] = useState("");
  const [passwordDuplicate, setPasswordDuplicate] = useState("");
  const [nickname, setNickname] = useState("");

  const [loginDuplicate, setLoginDuplicate] = useState(0);
  const [nicknameDuplicate, setNicknameDuplicate] = useState(0);
  const [passwordOk, setPasswordOk] = useState(0);
  const [passwordCheck, setPasswordCheck] = useState(0);

  const checkIdDuplicate = (e) => {
    e.preventDefault();
    axiostInstance
      .get(`user/id-check/${userid}`)
      .then((res) => {
        console.log(res);
        setLoginDuplicate(1);
        console.log(loginDuplicate);
      })
      .catch((err) => {
        setLoginDuplicate(-1);
        console.log(err);
      });
  };

  const checkNicknameDuplicate = (e) => {
    e.preventDefault();
    axiostInstance
      .get(`user/nickname-check/${nickname}`)
      .then((res) => {
        console.log(res);
        setNicknameDuplicate(1);
      })
      .catch((err) => {
        console.log(err);
        setNicknameDuplicate(-1);
      });
  };

  const checkPasswordOk = (e) => {
    setPassword(e.target.value);
    var regexPw = /^[A-Za-z0-9]{7,20}$/;

    if (regexPw.test(password)) {
      setPasswordOk(1);
    } else {
      setPasswordOk(-1);
    }
  };

  const checkPassword = (e) => {
    setPasswordDuplicate(e.target.value);

    if (password === e.target.value) {
      setPasswordCheck(1);
    } else {
      setPasswordCheck(-1);
    }
  };

  const doSignup = (e) => {
    e.preventDefault();

    if (loginDuplicate !== 1) {
      alert("아이디 중복체크를 해주세요!");
      return;
    }

    if (nicknameDuplicate !== 1) {
      alert("닉네임 중복체크를 해주세요!");
      return;
    }

    if (passwordOk !== 1) {
      alert("비밀번호는 8~20자 영문 대소문자, 숫자를 사용하세요.");
      return;
    }

    if (passwordCheck !== 1) {
      alert("비밀번호가 같지 않습니다!");
      return;
    }

    const signUpDto = {
      loginId: userid,
      nickname: nickname,
      password: password,
    };

    axiostInstance
      .post("user/signup", signUpDto)
      .then((res) => {
        console.log("성공");
        togglePage();
      })
      .catch((err) => {
        console.log(err);
      });
  };

  return (
    <div className="joinWrap">
      <Carousel />
      <div className={"signup-div"}>
        <h2>회원가입</h2>
        <div className="joinFormDiv">
          <form className="joinForm" onSubmit={doSignup}>
            <div className={className_inputContainer}>
              <input
                type={"text"}
                value={userid}
                onChange={(e) => {
                  setUserid(e.target.value);
                  setLoginDuplicate(0);
                }}
                className="inputBox"
              />
              <label className="labels">아이디</label>
              <motion.button
                className="checkBtn"
                onClick={checkIdDuplicate}
                whileHover={{ scale: 1.1 }}
                transition={{ type: "spring", stiffness: 400, damping: 10 }}
              >
                중복확인
              </motion.button>
            </div>
            <div className="errorMsg">
              {loginDuplicate === 0 && <div className="tempDiv"></div>}
              {loginDuplicate === 1 && (
                <div className="YesMsg">사용 가능한 아이디입니다!</div>
              )}
              {loginDuplicate === -1 && (
                <div className="NoMsg">사용할 수 없는 아이디입니다!</div>
              )}
            </div>
            <div className={className_inputContainer}>
              <input
                type={"password"}
                value={password}
                onChange={checkPasswordOk}
                className="passInput"
                style={{ width: "490px" }}
              />
              <label className="labels">비밀번호</label>
            </div>
            <div className="errorMsg">
              {passwordOk === 0 && <div className="tempDiv"></div>}
              {passwordOk === 1 && (
                <div className="YesMsg">사용가능한 비밀번호입니다.</div>
              )}
              {passwordOk === -1 && (
                <div className="NoMsg">
                  비밀번호는 8~20자 영문 대소문자, 숫자를 사용하세요.
                </div>
              )}
            </div>
            <div className={className_inputContainer}>
              <input
                type={"password"}
                value={passwordDuplicate}
                onChange={checkPassword}
                className="passInput"
                style={{ width: "490px" }}
              />
              <label className="labels">비밀번호 재입력</label>
            </div>
            <div className="errorMsg">
              {passwordCheck === 0 && <div className="tempDiv"></div>}
              {passwordCheck === 1 && (
                <div className="YesMsg">비밀번호가 일치합니다!</div>
              )}
              {passwordCheck === -1 && (
                <div className="NoMsg">비밀번호가 일치하지 않습니다</div>
              )}
            </div>
            <div className={className_inputContainer}>
              <input
                type={"text"}
                value={nickname}
                onChange={(e) => {
                  setNickname(e.target.value);
                  setNicknameDuplicate(0);
                }}
              />
              <label className="labels">닉네임</label>
              <motion.button
                className="checkBtn"
                onClick={checkNicknameDuplicate}
                whileHover={{ scale: 1.1 }}
                transition={{ type: "spring", stiffness: 400, damping: 10 }}
              >
                중복확인
              </motion.button>
            </div>
            <div className="errorMsg">
              {nicknameDuplicate === 0 && <div className="tempDiv"></div>}
              {nicknameDuplicate === 1 && (
                <div className="YesMsg">사용 가능한 닉네임입니다!</div>
              )}
              {nicknameDuplicate === -1 && (
                <div className="NoMsg">사용할 수 없는 닉네임입니다!</div>
              )}
            </div>
            <div style={{ justifyContent: "center" }}>
              <motion.button
                type={"submit"}
                style={{ width: "30%" }}
                className="joinBtn"
                whileHover={{ scale: 1.1 }}
                transition={{ type: "spring", stiffness: 400, damping: 10 }}
              >
                회원가입
              </motion.button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
};

export default SignupComponent;

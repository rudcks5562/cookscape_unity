import { useState } from "react";
import { motion } from "framer-motion";
import { useSelector } from "react-redux";

import "../css/Main.css";

import MainContent from "./MainContent";
import Description from "./Description";
import SignupComponent from "./SignupComponent";
import VideoPopUp from "./VideoPopUp";

import logo from "../assets/logo.png";
import user from "../assets/user.png";
import instagram from "../assets/instagram.png";
import youtube from "../assets/youtube.png";
import discord from "../assets/discord.png";

const Main = () => {
  const [doSignup, setDoSignup] = useState(false);

  const toggleSignupPage = () => {
    setDoSignup(!doSignup);
  };

  const popupOk = useSelector((state) => state.popup.isOk);

  return (
    <div className="App">
      <header className="App-header">
        <a href="https://j8b109.p.ssafy.io/">
          <motion.img
            whileHover={{ scale: 1.1 }}
            transition={{ type: "spring", stiffness: 400, damping: 10 }}
            src={logo}
            alt="로고"
            onClick={() => {
              setDoSignup(false);
            }}
            className="logoImg"
          />
        </a>
        <div className={`div-content`}>
          <motion.div
            whileHover={{ scale: 1.1 }}
            transition={{ type: "spring", stiffness: 400, damping: 10 }}
            onClick={() => {
              setDoSignup(true);
            }}
            className="joinUs"
          >
            <div className="joinMsg">
              <p>회원가입 하러가기</p>
            </div>
            <div>
              <img className="userImg" src={user} alt="" />
            </div>
          </motion.div>
        </div>
      </header>
      <section>
        {doSignup ? (
          <SignupComponent togglePage={toggleSignupPage} />
        ) : (
          <>
            <div className="mainSection">{popupOk ? <VideoPopUp /> : ""}</div>
            <MainContent />
            <Description />
          </>
        )}
      </section>
      <footer className={"footer"}>
        <div className="social">
          <img src={instagram} alt="" className="instagram" />
          <img src={youtube} alt="" className="youtube" />
          <img src={discord} alt="" className="discord" />
        </div>
        <hr />
        <div className="production">
          <p style={{ fontSize: "20px", marginTop: "10px" }}>
            <b>
              <i>@Cookscape</i>
            </b>
          </p>
          <p style={{ fontSize: "15px", marginTop: "10px" }}>
            <i>
              Production Team: B109(x를 눌러조) 김이슬 서현경 이정현 이지우
              이진혁 임경찬
            </i>
          </p>
        </div>
      </footer>
    </div>
  );
};

export default Main;

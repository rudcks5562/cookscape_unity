import React, { useState, useEffect } from "react";
import { motion } from "framer-motion";

import tomato from "../assets/tomato.png";

import "../css/MainTopButton.css";

const MainTopButton = () => {
  const [showBtn, setShowBtn] = useState(false);

  const scrollToTop = () => {
    window.scroll({
      top: 0,
      behavior: "smooth",
    });
  };

  useEffect(() => {
    const ShowBtnClick = () => {
      if (window.scrollY > 300) {
        setShowBtn(true);
      } else {
        setShowBtn(false);
      }
    };
    window.addEventListener("scroll", ShowBtnClick);
    return () => {
      window.removeEventListener("scroll", ShowBtnClick);
    };
  }, []);

  return (
    <>
      {showBtn && (
        <div className="topButtonDiv">
          <div onClick={scrollToTop} className="scrollTopButton">
            <motion.img
              whileHover={{ scale: 1.1 }}
              transition={{ type: "spring", stiffness: 400, damping: 10 }}
              src={tomato}
              alt=""
              className="tomatos"
            />
          </div>
        </div>
      )}
    </>
  );
};

export default MainTopButton;

import { axiostInstance } from "../utils/getAxios";
import { motion } from "framer-motion";
import { useState, useRef } from "react";
import { useDispatch, useSelector } from "react-redux";
import { popupActions } from "../store/popup";

import "../css/DownloadButton.scss";

const DownloadFileButton = () => {
  const [msg, SetMsg] = useState("Download");
  const downloadBtn = useRef();
  const [clickDownload, setClickDownload] = useState(false);
  const dispatch = useDispatch();

  const popupOk = useSelector((state) => state.popup.isOk);

  const Test = () => {
    dispatch(popupActions.popupOk());
    console.log(popupOk);
  };

  const downloadZipFile = async (e) => {
    try {
      Test();
      SetMsg("Downloading...");

      // 파일의 URL을 설정합니다. 'public' 폴더 내부의 파일에 접근할 때는 %PUBLIC_URL%을 사용합니다.
      const fileUrl = `${process.env.PUBLIC_URL}/Cookscape_1_1.zip`;

      // a 태그를 생성하고, 파일 URL을 href 속성에 할당합니다.
      const link = document.createElement("a");
      link.href = fileUrl;

      // 파일의 이름을 설정합니다. (예: large-file.zip)
      link.download = "Cookscape.zip";

      // a 태그를 문서에 추가하고, 클릭 이벤트를 트리거합니다.
      document.body.appendChild(link);
      link.click();

      // 다운로드가 시작된 후 a 태그를 문서에서 제거합니다.
      document.body.removeChild(link);

      SetMsg("Download");
      setClickDownload(true);
      e.target.classList.add("animate");
    } catch (error) {
      console.error("Error downloading file:", error);
    }
  };

  var animateButton = function (e) {
    e.preventDefault();
    //reset animation
    e.target.classList.remove("animate");

    e.target.classList.add("animate");
    setTimeout(function () {
      e.target.classList.remove("animate");
    }, 500);
  };

  var bubblyButtons = document.getElementsByClassName("bubbly-button");

  for (var i = 0; i < bubblyButtons.length; i++) {
    bubblyButtons[i].addEventListener("click", animateButton, false);
  }

  return (
    <motion.div
      whileHover={{ scale: 1.1 }}
      transition={{ type: "spring", stiffness: 400, damping: 10 }}
    >
      <button
        ref={downloadBtn}
        onClick={downloadZipFile}
        className="bubbly-button"
      >
        {msg}
      </button>
    </motion.div>
  );
};

export default DownloadFileButton;

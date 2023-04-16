import { motion } from "framer-motion";
import DownloadFileButton from "./GameDownloadButtonComponent";
import "../css/MainContent.css";

const MainContent = () => {
  return (
    <>
      <div className={"main-page"} id="download">
        <div id="download_msg">
          <p>지금 다운로드 하기</p>
        </div>
        <DownloadFileButton />
      </div>
    </>
  );
};

export default MainContent;

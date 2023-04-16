import useScrollFadeIn from "../fucntion/UseScrollFadeIn";

import background1 from "../assets/background_logo_tomato.png";
import background2 from "../assets/background_logo_meat.png";
import background3 from "../assets/background_logo_pimang.png";
import background4 from "../assets/background_logo_coke.png";

import "../css/Description.css";

const Description = () => {
  const animatedItem = {
    0: useScrollFadeIn("right", 1.1, 0.2),
    1: useScrollFadeIn("left", 1.1, 0.2),
    2: useScrollFadeIn("right", 1.1, 0.2),
    3: useScrollFadeIn("left", 1.1, 0.2),
  };

  return (
    <>
      <div className="section section1">
        <div>
          {/* <img
            {...animatedItem[0]}
            src={background1}
            alt=""
            className="background Img"
          /> */}
          <video loop muted autoPlay className="background Img">
            <source
              src={`${process.env.PUBLIC_URL}/video/trailer_tomato_mp4.mp4`}
              type="video/mp4"
            />
          </video>
        </div>
        <div {...animatedItem[0]} className="titles">
          <p className="descriptions title">친구들과 함께</p>
          <p className="descriptions title">
            <span>쿠킹아일랜드</span>를 즐겨봐요 !
          </p>
          <p className="descriptions cont1">
            메타버스에서 여유로운 휴가를 즐겨보는 건 어때요 ?
          </p>
          <p className="descriptions content">
            쿠킹아일랜드에도 봄이 찾아왔답니다 🌸
          </p>
          <p className="descriptions content">
            이번 여름휴가는 여기서 보내봐요 :)
          </p>
        </div>
      </div>
      <div className="section section2">
        <div {...animatedItem[1]} className="titles">
          <p
            className="descriptions title"
            style={{ textShadow: "1px 1px 1px #414141", color: "#000" }}
          >
            <span style={{ color: "#00ca65" }}>무시무시한 요리사</span>를 피해서
          </p>
          <p
            className="descriptions title"
            style={{ textShadow: "1px 1px 1px #414141", color: "#000" }}
          >
            다같이<span style={{ color: "#00ca65" }}> 주방</span>을 탈출해요 💥
          </p>
          <p
            className="descriptions cont1"
            style={{ textShadow: "1px 1px 1px #727272", color: "#000" }}
          >
            식재료는 요리사를 피해 도망가야해요 !
          </p>
          <p
            className="descriptions content"
            style={{ textShadow: "1px 1px 1px #727272", color: "#000" }}
          >
            요리사에게 잡히면 싱크대에서 세척됩니다 💦
          </p>
          <p
            className="descriptions content"
            style={{ textShadow: "1px 1px 1px #727272", color: "#000" }}
          >
            젓가락을 사용해 친구들을 구하러 가요 😎
          </p>
        </div>
        <div>
          <video loop muted autoPlay className="background Img1">
            <source
              src={`${process.env.PUBLIC_URL}/video/ingame1.mp4`}
              type="video/mp4"
            />
          </video>
        </div>
      </div>
      <div className="section section3">
        <div>
          <video loop muted autoPlay className="background Img">
            <source
              src={`${process.env.PUBLIC_URL}/video/trailer_foods_mp4.mp4`}
              type="video/mp4"
            />
          </video>
        </div>
        <div {...animatedItem[2]} className="titles">
          <p className="descriptions title">날씨도 좋은데,</p>
          <p className="descriptions title">
            <span>쿡스랜드</span>는 어떤가요 ?
          </p>
          <p className="descriptions cont1">짜릿한 롤러코스터를 타고 🎢</p>
          <p className="descriptions content">
            관람차로 쿠킹아일랜드를 한눈에 담아보아요 🎡
          </p>
          <p className="descriptions content">
            다양한 어트렉션을 친구들과 함께 즐길 수 있습니다
          </p>
        </div>
      </div>
      <div className="section section4">
        <div {...animatedItem[3]} className="titles">
          <p
            className="descriptions title"
            style={{ textShadow: "1px 1px 1px #414141", color: "#000" }}
          >
            배가 고프시다고요?
          </p>
          <p
            className="descriptions title"
            style={{ textShadow: "1px 1px 1px #414141", color: "#000" }}
          >
            오늘은 내가 <span style={{ color: "#00ca65" }}>요리사</span> 🍴
          </p>
          <p
            className="descriptions cont1"
            style={{ textShadow: "1px 1px 1px #727272", color: "#000" }}
          >
            쿡스케이프에서는 요리사로도 게임이 가능합니다 :)
          </p>
          <p
            className="descriptions content"
            style={{ textShadow: "1px 1px 1px #727272", color: "#000" }}
          >
            식재료 친구들을 마구마구 잡아서
          </p>
          <p
            className="descriptions content"
            style={{ textShadow: "1px 1px 1px #727272", color: "#000" }}
          >
            맛있는 한끼를 만들어 보아요 👨🏻‍🍳
          </p>
        </div>
        <div>
          <video loop muted autoPlay className="background Img3">
            <source
              src={`${process.env.PUBLIC_URL}/video/ingame2.mp4`}
              type="video/mp4"
            />
          </video>
        </div>
      </div>
    </>
  );
};

export default Description;

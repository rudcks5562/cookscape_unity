import React from "react";
// Import Swiper React components
import { Swiper, SwiperSlide } from "swiper/react";

// Import Swiper styles
import "swiper/css";
import "swiper/css/pagination";
// import "swiper/css/navigation";

import "../css/Carousel.css";

import background1 from "../assets/background_logo_coke.png";
import background2 from "../assets/background_logo_meat.png";
import background3 from "../assets/background_logo_pimang.png";
import background4 from "../assets/background_logo_tomato.png";

// import required modules
import { Autoplay, Pagination } from "swiper";

const Carousel = () => {
  return (
    <div className={"carousel"}>
      <Swiper
        spaceBetween={30}
        centeredSlides={true}
        autoplay={{
          delay: 4000,
          disableOnInteraction: false,
        }}
        pagination={{
          clickable: false,
        }}
        navigation={true}
        modules={[Autoplay, Pagination]}
        className="mySwiper"
      >
        <SwiperSlide>
          <img src={background1} alt="" />
        </SwiperSlide>
        <SwiperSlide>
          <img src={background2} alt="" />
        </SwiperSlide>
        <SwiperSlide>
          <img src={background3} alt="" />
        </SwiperSlide>
        <SwiperSlide>
          <img src={background4} alt="" />
        </SwiperSlide>
      </Swiper>
    </div>
  );
};

export default Carousel;

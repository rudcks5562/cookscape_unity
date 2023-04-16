package com.ssafy.cookscape.global.util;

import com.ssafy.cookscape.global.common.model.response.SuccessResponse;
import org.springframework.http.HttpStatus;
import org.springframework.stereotype.Component;

import java.time.LocalDateTime;
import java.time.ZonedDateTime;
import java.util.TimeZone;

@Component
public class ResponseUtil<T> {

	/**
	 * 요청에 성공적으로 응답했을 때 SuccessResponse 리턴합니다.
	 *
	 * @param body response에 담아서 보낼 데이터
	 * @return SuccessResponse
	 */
	public SuccessResponse<T> buildSuccessResponse(T body) {
		return SuccessResponse.<T>builder()
				.timestamp(LocalDateTime.now())
				.body(body)
				.build();
	}
}

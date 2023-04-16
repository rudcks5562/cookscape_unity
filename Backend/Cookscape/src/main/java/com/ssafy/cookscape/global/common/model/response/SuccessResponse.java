package com.ssafy.cookscape.global.common.model.response;

import com.fasterxml.jackson.annotation.JsonFormat;
import com.ssafy.cookscape.global.error.ErrorResponse;
import com.ssafy.cookscape.global.error.exception.ErrorCode;
import lombok.*;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;

import java.time.LocalDateTime;
import java.time.ZonedDateTime;

@Getter
@Builder
public class SuccessResponse<T> {

	private LocalDateTime timestamp;
	private T body;
}
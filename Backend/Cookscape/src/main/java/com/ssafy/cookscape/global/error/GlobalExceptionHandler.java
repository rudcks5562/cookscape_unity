package com.ssafy.cookscape.global.error;

import com.ssafy.cookscape.global.error.exception.CustomException;
import com.ssafy.cookscape.global.error.exception.ErrorCode;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.ExceptionHandler;
import org.springframework.web.bind.annotation.RestControllerAdvice;
import org.springframework.web.servlet.mvc.method.annotation.ResponseEntityExceptionHandler;

@Slf4j
@RestControllerAdvice
@RequiredArgsConstructor
public class GlobalExceptionHandler extends ResponseEntityExceptionHandler {

	/**
	 * 사용자 지정 에러에 대한 내용을 포함한 ResponseEntity를 리턴합니다.
	 *
	 * @param e 에러 내용을 포함하고 있는 CustomException
	 * @return ResponseEntity
	 */
	@ExceptionHandler(value = {CustomException.class})
	private ResponseEntity<ErrorResponse> handleCustomException(CustomException e) {
		log.error("handleCustomException throw CustomException : {}", e.getErrorcode());
		return ErrorResponse.toResponseEntity(e.getErrorcode());
	}

	/**
	 * 서버 에러에 대한 내용을 포함한 ResponseEntity를 리턴합니다.
	 *
	 * @param e 에러 내용을 포함하고 있는 Exception
	 * @return ResponseEntity
	 */
	@ExceptionHandler(value = {Exception.class})
	protected ResponseEntity<ErrorResponse> handleServerException(Exception e) {
		log.error("handleServerException throw ServerError : {}", e.getMessage());
		return ErrorResponse.toResponseEntity(ErrorCode.INTERNAL_SERVER_ERROR);
	}
}

/*
 * 정적 리소스 (resources) 디렉토리에 직접 접근해야 할 경우 해당 코스를 수정해야 됨 404가 안뜸
 */

package com.ssafy.cookscape.global.config;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.annotation.Configuration;
import org.springframework.web.servlet.config.annotation.ResourceHandlerRegistry;
import org.springframework.web.servlet.config.annotation.WebMvcConfigurer;

@Configuration
public class WebConfig implements WebMvcConfigurer {

	@Value("${file.app.url}")
	private String APP_URL;

	@Value("${file.app.windowPath}")
	private String WINDOW_PATH;

	@Override
	public void addResourceHandlers(ResourceHandlerRegistry registry) {
		registry.addResourceHandler(APP_URL)
				.addResourceLocations(WINDOW_PATH);
	}
}

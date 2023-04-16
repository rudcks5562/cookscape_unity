FROM openjdk:11.0.11-jre-slim

WORKDIR /jar

COPY ["Cookscape.jar", "."]

ARG ARG_PROFILE
ENV PROFILE=${ARG_PROFILE}

ENTRYPOINT ["java", "-Dspring.profiles.active=${PROFILE}", "-Duser.timezone=Asia/Seoul", "-jar","Cookscape.jar"]

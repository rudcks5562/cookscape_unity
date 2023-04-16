FROM node:16.16.0-alpine as build
WORKDIR /app

# 패키지 설치 단계
COPY ["./package.json", "."]
COPY ["./package-lock.json", "."]
RUN ["npm", "install"]

# 빌드 단계
COPY ["." , "."]

EXPOSE 3000

CMD ["npm", "start"]


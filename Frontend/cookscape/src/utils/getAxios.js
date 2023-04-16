import axios from "axios";

export const axiostInstance = axios.create({
  baseURL:
    process.env.NODE_ENV === "development"
      ? //"https://localhost:9999/api/v1/"
        "https://j8b109.p.ssafy.io:9999/api/v1"
      : "https://j8b109.p.ssafy.io:9999/api/v1",
  headers: {
    "Content-Type": "application/json",
  },
});

import axios from "axios";

const API = axios.create({
    baseURL: "https://localhost:7048/api"
});

export const analyzeSentiment = (text) =>
    API.post("/Sentiment", { text });
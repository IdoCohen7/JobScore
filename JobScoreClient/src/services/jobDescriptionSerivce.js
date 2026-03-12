import api from "../api/axios";

const jobDescriptionService = {
  Create: (title, content) => {
    return api.post("/api/JobDescription", {
      title,
      content,
    });
  },
  Evaluate: (title, content) => {
    return api.post("/api/JobDescription/evaluate", {
      title,
      content,
    });
  },
};

export default jobDescriptionService;

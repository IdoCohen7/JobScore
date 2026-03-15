import api from "../api/axios";

const buzzwordService = {
  GetBuzzwords: () => {
    return api.get("/api/Buzzword");
  },
  CreateBuzzword: (name) => {
    return api.post("/api/Buzzword", name);
  },
  DeleteBuzzword: (id) => {
    return api.delete(`/api/Buzzword/${id}`);
  },
};

export default buzzwordService;

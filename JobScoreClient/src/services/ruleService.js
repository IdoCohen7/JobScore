import api from "../api/axios";

const ruleService = {
  GetRules: () => {
    return api.get("/api/Rule");
  },
  UpdateRulesWeight: (rules) => {
    if (!Array.isArray(rules)) {
      throw new Error("UpdateRulesWeight expects an array of { id, weight }");
    }

    return api.put("/api/Rule/weights", rules);
  },
};

export default ruleService;

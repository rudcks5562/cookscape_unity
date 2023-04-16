import { Route, Routes, Navigate } from "react-router-dom";

import Main from "./components/Main";
import "./App.css";

const App = () => {
  return (
    <Routes>
      <Route path="/" element={<Main />} />
    </Routes>
  );
};

export default App;

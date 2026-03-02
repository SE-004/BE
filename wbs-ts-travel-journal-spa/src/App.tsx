import { BrowserRouter, Routes, Route } from "react-router";
import { RootLayout } from "@/layouts";
import { CreatePost, Home, Login, NotFound, Post, Register } from "@/pages";
import { RequireAuth, RequireGuest } from "./routeGuard";

const App = () => (
  <BrowserRouter>
    <Routes>
      <Route path="/" element={<RootLayout />}>
        <Route index element={<Home />} />
        <Route
          path="login"
          element={
            <RequireGuest>
              <Login />
            </RequireGuest>
          }
        />
        <Route
          path="register"
          element={
            <RequireGuest>
              <Register />
            </RequireGuest>
          }
        />
        <Route path="post/:id" element={<Post />} />
        <Route
          path="create"
          element={
            <RequireAuth>
              <CreatePost />
            </RequireAuth>
          }
        />
        <Route path="*" element={<NotFound />} />
      </Route>
    </Routes>
  </BrowserRouter>
);

export default App;

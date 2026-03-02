import { useEffect, useState } from "react";
import { toast } from "react-toastify";
import { getPosts } from "@/data";
import { PostCard, PostsSkeleton } from "@/components";

const Home = () => {
  const [loading, setLoading] = useState(true);
  const [posts, setPosts] = useState<Post[]>([]);

  useEffect(() => {
    (async () => {
      try {
        const fetchedPosts: Post[] = await getPosts();
        setPosts(fetchedPosts);
      } catch (error: unknown) {
        const message = (error as { message: string }).message;
        toast.error(message);
      } finally {
        setLoading(false);
      }
    })(); // IIFE - Immediately Invoked Function Expression
  }, []);

  console.log(posts);

  if (loading) return <PostsSkeleton />;
  return (
    <div className="grid grid-cols-1 lg:grid-cols-4 gap-4 ">
      {posts.length ? (
        posts.map((post) => (
          <PostCard
            key={post._id}
            _id={post._id}
            content={post.content}
            image={post.image}
            title={post.title}
          />
        ))
      ) : (
        <span className="text-center w-screen">No posts in the DB yet</span>
      )}
    </div>
  );
};

export default Home;

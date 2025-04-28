import { PostModel } from "./post.model";

export interface PostsModel {
  posts: PostModel[];
  totalCount: number;
}

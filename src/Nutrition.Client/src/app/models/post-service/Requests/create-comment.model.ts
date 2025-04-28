export interface CreateCommentModel {
  postId: string;
  ownerEmail: string | null;
  ownerId: string | null;
  text: string;
}

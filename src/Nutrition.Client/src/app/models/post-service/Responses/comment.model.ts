export interface CommentModel {
  id: string;
  ownerEmail: string;
  date: Date;
  text: string;
  amountOfLikes: number;
  isLiked: boolean;
  isOwner: boolean;
}

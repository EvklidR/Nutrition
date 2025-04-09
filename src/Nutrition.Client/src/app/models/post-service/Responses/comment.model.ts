export interface CommentModel {
  id: string;
  ownerEmail: string;
  date: Date;
  text: string;
  AmountOfLikes: number;
  isLiked: boolean;
}

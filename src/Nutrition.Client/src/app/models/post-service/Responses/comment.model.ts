export interface CommentModel {
  id: string;
  ownerEmail: string;
  creationDate: Date;
  text: string;
  amountOfLikes: number;
  isLiked: boolean;
  isOwner: boolean;
  isEdited: boolean;
}

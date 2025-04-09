export interface PostModel {
  id: string;
  title: string;
  text: string;
  creationDate: Date;
  ownerEmail: string;
  keyWords: string[];
  amountOfLikes: number;
  amountOfComments: number;
  isLiked: boolean;
}

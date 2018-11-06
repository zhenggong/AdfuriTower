//
//  MovieReward6010.h(Afio)
//
//  Copyright (c) A .D F. U. L. L. Y Co., Ltd. All rights reserved.
//
//

#import <Foundation/Foundation.h>
#import <ADFMovieReward/ADFmyMovieRewardInterface.h>
#import <ADFMovieReward/ADFmyMovieDelegateBase.h>
#import "AMoAdInterstitial.h"
#import "AMoAdInterstitialVideo.h"
#import "AMoAd.h"

@interface MovieReward6010 : ADFmyMovieRewardInterface<AMoAdInterstitialVideoDelegate>

@property (nonatomic) AMoAdInterstitialVideo *amoadInterstitialVideo;

-(void)setCancellable;

@end
